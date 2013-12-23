namespace RapidTransit.Integration
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core;
    using MassTransit;
    using MassTransit.BusConfigurators;


    /// <summary>
    /// Configures the transport for a service bus instance to use RabbitMQ, including
    /// the ReceiveFrom address
    /// </summary>
    public class RabbitMqTransportConfigurator :
        ITransportConfigurator
    {
        static readonly Regex _prefetch = new Regex(@"([\?\&])prefetch=[^\&]+[\&]?");
        readonly RabbitMqSettings _settings;

        public RabbitMqTransportConfigurator(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        void ITransportConfigurator.Configure(ServiceBusConfigurator configurator, string queueName, int? consumerLimit)
        {
            Uri receiveFrom = GetQueueAddress(queueName, consumerLimit);

            configurator.UseRabbitMq(x =>
                {
                    x.ConfigureHost(receiveFrom, h =>
                        {
                            h.SetUsername(_settings.Username);
                            h.SetPassword(_settings.Password);
                            h.SetRequestedHeartbeat(_settings.Heartbeat);
                        });
                });

            configurator.ReceiveFrom(receiveFrom);
            if (consumerLimit.HasValue)
                configurator.SetConcurrentConsumerLimit(consumerLimit.Value);
        }

        public Uri GetQueueAddress(string queueName, int? consumerLimit = default(int?))
        {
            string[] paths = _settings.VirtualHost.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Join("/", paths.Concat(new[] {queueName}).ToArray());

            var builder = new UriBuilder("rabbitmq", _settings.Host)
                {
                    Path = path,
                    Query = _settings.Options
                };

            if (consumerLimit.HasValue)
                return SetPrefetchCount(builder.Uri, consumerLimit.Value);

            return builder.Uri;
        }

        Uri SetPrefetchCount(Uri uri, int consumerLimit)
        {
            string query = uri.Query;

            if (query.IndexOf("prefetch", StringComparison.InvariantCultureIgnoreCase) >= 0)
                query = _prefetch.Replace(query, string.Format("prefetch={0}", consumerLimit));
            else if (string.IsNullOrEmpty(query))
                query = string.Format("prefetch={0}", consumerLimit);
            else
                query += string.Format("&prefetch={0}", consumerLimit);

            var builder = new UriBuilder(uri)
                {
                    Query = query.Trim('?')
                };

            return builder.Uri;
        }
    }
}