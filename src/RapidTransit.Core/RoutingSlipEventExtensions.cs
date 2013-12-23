namespace RapidTransit.Core
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;
    using MassTransit.Serialization;
    using Newtonsoft.Json.Linq;


    public static class RoutingSlipEventExtensions
    {
        public static T GetVariable<T>(this RoutingSlipCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializerVariable<T>(key, source.Variables);
        }

        public static string GetVariable(this RoutingSlipCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            object obj;
            if (!source.Variables.TryGetValue(key, out obj))
                throw new KeyNotFoundException("The variable was not present: " + key);

            return obj as string;
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializerVariable<T>(key, source.Variables);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializerVariable<T>(key, source.Results);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializerVariable<T>(key, source.Results);
        }

        static T DeserializerVariable<T>(string key, IDictionary<string, object> dictionary)
        {
            object obj;
            if (!dictionary.TryGetValue(key, out obj))
                throw new KeyNotFoundException("The variable was not present: " + key);

            JToken token = obj as JToken ?? new JObject();

            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
                return (T)JsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(T));
        }
    }
}