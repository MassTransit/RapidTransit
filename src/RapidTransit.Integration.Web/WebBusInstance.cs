namespace RapidTransit.Integration.Web
{
    using Core.Configuration;
    using Core.Services;


    public abstract class WebBusInstance :
        ServiceBusInstance
    {
        protected WebBusInstance(IConfigurationProvider configuration, string queueKey, string consumerLimitKey,
            int defaultConsumerLimit)
            : base(configuration, queueKey, consumerLimitKey, defaultConsumerLimit)
        {
        }
    }
}