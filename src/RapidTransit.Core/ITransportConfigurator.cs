namespace RapidTransit.Core
{
    using System;
    using MassTransit.BusConfigurators;


    /// <summary>
    /// Configures a service bus for a specific transport
    /// </summary>
    public interface ITransportConfigurator
    {
        /// <summary>
        /// Configure a service bus instance using the queue name and consumer limit specified
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="queueName"></param>
        /// <param name="consumerLimit"></param>
        void Configure(ServiceBusConfigurator configurator, string queueName, int? consumerLimit = default(int?));

        /// <summary>
        /// Return the Uri for a queue on the transport
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="consumerLimit"></param>
        /// <returns></returns>
        Uri GetQueueAddress(string queueName, int? consumerLimit = default(int?));
    }
}