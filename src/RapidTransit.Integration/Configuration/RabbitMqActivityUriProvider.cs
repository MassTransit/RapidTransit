namespace RapidTransit.Integration.Configuration
{
    using System;
    using Core;
    using Core.Configuration;


    public class RabbitMqActivityUriProvider :
        IActivityUriProvider
    {
        readonly IConfigurationProvider _configurationProvider;

        readonly RabbitMqSettings _settings;


        public RabbitMqActivityUriProvider(IConfigurationProvider configurationProvider, RabbitMqSettings settings)
        {
            _configurationProvider = configurationProvider;

            _settings = settings;
        }

        public Uri GetExecuteActivityUri(string activityName)
        {
            string queueName = GetExecuteActivityQueueName(activityName);

            return _settings.GetQueueAddress(queueName);
        }

        public Uri GetCompensateActivityUri(string activityName)
        {
            string queueName = GetCompensateActivityQueueName(activityName);

            return _settings.GetQueueAddress(queueName);
        }

        public string GetExecuteActivityQueueName(string activityName)
        {
            string prefix = _configurationProvider.GetSetting("HAQueuePrefix", "");

            return string.Format("{1}execute_{0}", activityName.ToLowerInvariant(), prefix);
        }

        public string GetCompensateActivityQueueName(string activityName)
        {
            string prefix = _configurationProvider.GetSetting("HAQueuePrefix", "");

            return string.Format("{1}compensate_{0}", activityName.ToLowerInvariant(), prefix);
        }
    }
}