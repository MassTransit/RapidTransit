namespace RapidTransit.Core
{
    using System;
    using System.Linq;


    public static class RabbitMqSettingsExtensions
    {
        public static Uri GetQueueAddress(this RabbitMqSettings settings, string queueName)
        {
            string[] paths = settings.VirtualHost.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Join("/", paths.Concat(new[] {queueName}).ToArray());

            var builder = new UriBuilder("rabbitmq", settings.Host, settings.Port)
                {
                    Path = path,
                    Query = settings.Options
                };

            return builder.Uri;
        }
    }
}