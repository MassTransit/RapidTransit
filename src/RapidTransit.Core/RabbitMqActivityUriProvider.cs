namespace RapidTransit.Core
{
    using System;


    public class RabbitMqActivityUriProvider :
        IActivityUriProvider
    {
        readonly RabbitMqSettings _settings;

        public RabbitMqActivityUriProvider(RabbitMqSettings settings)
        {
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
            return string.Format("{1}execute_{0}", activityName.ToLowerInvariant(),
                _settings.HighAvailabilityQueuePrefix);
        }

        public string GetCompensateActivityQueueName(string activityName)
        {
            return string.Format("{1}compensate_{0}", activityName.ToLowerInvariant(),
                _settings.HighAvailabilityQueuePrefix);
        }
    }
}