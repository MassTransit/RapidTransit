namespace RapidTransit.Integration
{
    using System;
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
        readonly RabbitMqSettings _settings;

        public RabbitMqTransportConfigurator(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        void ITransportConfigurator.Configure(ServiceBusConfigurator configurator, string queueName, int? consumerLimit)
        {
            Uri receiveFrom = _settings.GetQueueAddress(queueName, consumerLimit);

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
            return _settings.GetQueueAddress(queueName, consumerLimit);
        }
    }
}