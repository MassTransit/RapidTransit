namespace RapidTransit.Integration.Web
{
    using Core.Configuration;
    using Core.Services;


    public abstract class WebBusInstance :
        ServiceBusInstance
    {
        protected WebBusInstance(IConfigurationProvider configurationProvider, string queueKey, string consumerLimitKey,
            int defaultConsumerLimit)
            : base(configurationProvider, queueKey, consumerLimitKey, defaultConsumerLimit)
        {
        }
    }
}