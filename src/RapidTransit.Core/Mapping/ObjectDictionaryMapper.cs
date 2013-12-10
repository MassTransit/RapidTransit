﻿namespace RapidTransit.Core.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ObjectDictionaryMapper<T> :
        IDictionaryMapper<T>
    {
        readonly IDictionaryConverter _converter;
        readonly ReadOnlyProperty<T> _property;

        public ObjectDictionaryMapper(ReadOnlyProperty<T> property, IDictionaryConverter converter)
        {
            _property = property;
            _converter = converter;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            IDictionary<string, object> value = _converter.GetDictionary(obj);

            dictionary.Add(_property.Property.Name, value);
        }
    }
}