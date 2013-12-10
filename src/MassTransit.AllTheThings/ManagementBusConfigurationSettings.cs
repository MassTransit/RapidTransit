namespace RapidTransit.Integration
{
    using System;
    using Core;
    using Core.Configuration;
    using Topshelf.Runtime;


    public class ManagementBusConfigurationSettings :
        ManagementBusSettings
    {
        readonly string _baseQueueName;

        public ManagementBusConfigurationSettings(IConfigurationProvider configurationProvider,
            HostSettings hostSettings)
        {
            _baseQueueName = GetBaseQueueName(configurationProvider, hostSettings);
        }

        public string QueueName
        {
            get { return string.Format("{0}_{1}", _baseQueueName, Environment.MachineName.ToLowerInvariant()); }
        }

        static string GetBaseQueueName(IConfigurationProvider configurationProvider, HostSettings hostSettings)
        {
            string baseQueueName;
            if (configurationProvider.TryGetSetting("ManagementBusQueueName", out baseQueueName))
                return baseQueueName;
            return hostSettings.ServiceName.Replace(" ", "_");
        }
    }
}