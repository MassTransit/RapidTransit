namespace RapidTransit.Core
{
    using MassTransit.BusConfigurators;


    /// <summary>
    /// Configures a service bus for a specific transport
    /// </summary>
    public interface ITransportConfigurator
    {
        void Configure(ServiceBusConfigurator configurator, string queueName, int? consumerLimit = default(int?));
    }
}