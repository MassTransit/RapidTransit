namespace RapidTransit.Integration.Services
{
    using System;
    using Core;
    using Core.Configuration;
    using Topshelf.Runtime;


    /// <summary>
    /// Registered in the container to implement the management bus settings
    /// </summary>
    public class HostServiceBusConfigurationSettings :
        HostServiceBusSettings
    {
        const string QueueNameKey = "HostServiceBusQueueName";

        readonly string _baseQueueName;

        public HostServiceBusConfigurationSettings(IConfigurationProvider configurationProvider,
            HostSettings hostSettings)
        {
            _baseQueueName = GetBaseQueueName(configurationProvider, hostSettings);
        }

        string HostServiceBusSettings.QueueName
        {
            get { return string.Format("{0}_{1}", _baseQueueName, Environment.MachineName.ToLowerInvariant()); }
        }

        static string GetBaseQueueName(IConfigurationProvider configurationProvider, HostSettings hostSettings)
        {
            string baseQueueName;
            if (configurationProvider.TryGetSetting(QueueNameKey, out baseQueueName))
                return baseQueueName;

            return hostSettings.ServiceName.Replace(" ", "_");
        }
    }
}