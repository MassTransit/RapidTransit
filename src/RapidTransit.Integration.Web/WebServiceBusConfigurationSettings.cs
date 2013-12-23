namespace RapidTransit.Integration.Web
{
    using System;
    using System.Web.Hosting;
    using Core;
    using Core.Configuration;


    /// <summary>
    /// Registered in the container to implement the management bus settings
    /// </summary>
    public class WebServiceBusConfigurationSettings :
        HostServiceBusSettings
    {
        const string QueueNameKey = "HostServiceBusQueueName";

        readonly string _baseQueueName;

        public WebServiceBusConfigurationSettings(IConfigurationProvider configurationProvider)
        {
            _baseQueueName = GetBaseQueueName(configurationProvider);
        }

        string HostServiceBusSettings.QueueName
        {
            get { return string.Format("{0}_{1}", _baseQueueName, Environment.MachineName.ToLowerInvariant()); }
        }

        static string GetBaseQueueName(IConfigurationProvider configurationProvider)
        {
            string baseQueueName;
            if (configurationProvider.TryGetSetting(QueueNameKey, out baseQueueName))
                return baseQueueName;

            return HostingEnvironment.SiteName.Replace(" ", "_");
        }
    }
}