namespace RapidTransit.Integration.Web
{
    using Core.Configuration;
    using Core.Services;
    using MassTransit.Logging;


    public abstract class WebBusHost :
        ServiceBusHost
    {
        static readonly ILog _log = Logger.Get<WebBusHost>();

        protected WebBusHost(IConfigurationProvider configuration, string queueKey, string consumerLimitKey,
            int defaultConsumerLimit)
            : base(configuration, queueKey, consumerLimitKey, defaultConsumerLimit)
        {
        }
    }
}