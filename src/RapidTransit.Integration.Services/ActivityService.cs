namespace RapidTransit.Integration.Services
{
    using System;
    using Configuration;
    using Core;
    using Core.Configuration;
    using MassTransit;
    using MassTransit.Courier;
    using MassTransit.Logging;
    using Topshelf;


    public class ActivityService<TActivity, TArguments, TLog> :
        ServiceControl,
        IDisposable
        where TActivity : Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly string _activityName;
        readonly IActivityUriProvider _activityUriProvider;
        readonly CompensateActivityFactory<TLog> _compensateActivityFactory;
        readonly int _compensateConsumerLimit;
        readonly string _compensateQueueName;
        readonly ExecuteActivityFactory<TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;
        readonly ILog _log;
        readonly ITransportConfigurator _transportFactory;
        IServiceBus _compensateBus;
        bool _disposed;
        IServiceBus _executeBus;

        public ActivityService(IConfigurationProvider configuration, ITransportConfigurator transportFactory,
            IActivityUriProvider activityUriProvider,
            ExecuteActivityFactory<TArguments> executeActivityFactory,
            CompensateActivityFactory<TLog> compensateActivityFactory)
        {
            _log = Logger.Get(GetType());

            _transportFactory = transportFactory;
            _activityUriProvider = activityUriProvider;
            _executeActivityFactory = executeActivityFactory;
            _compensateActivityFactory = compensateActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = _activityUriProvider.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);

            _compensateQueueName = _activityUriProvider.GetCompensateActivityQueueName(_activityName);
            _compensateConsumerLimit = GetCompensateConsumerLimit(configuration);
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            if (_executeBus != null)
                _executeBus.Dispose();
            if (_compensateBus != null)
                _compensateBus.Dispose();

            _disposed = true;
        }

        public virtual bool Start(HostControl hostControl)
        {
            _compensateBus = CreateCompensateServiceBus();

            _executeBus = CreateExecuteServiceBus(_compensateBus);

            return true;
        }

        public virtual bool Stop(HostControl hostControl)
        {
            if (_executeBus != null)
            {
                _log.InfoFormat("Stopping Execute {0} Service Bus", _activityName);
                _executeBus.Dispose();
                _executeBus = null;
            }

            if (_compensateBus != null)
            {
                _log.InfoFormat("Stopping Compensate {0} Service Bus", _activityName);
                _compensateBus.Dispose();
                _compensateBus = null;
            }

            return true;
        }

        string GetActivityName()
        {
            string activityName = typeof(TActivity).Name;
            if (activityName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - "Service".Length);
            return activityName;
        }

        int GetExecuteConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = string.Format("{0}ConsumerLimit", _activityName);

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount);
        }

        int GetCompensateConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = string.Format("{0}ConsumerLimit", _activityName);

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount / 2);
        }

        protected virtual IServiceBus CreateExecuteServiceBus(IServiceBus compensateServiceBus)
        {
            if (compensateServiceBus == null)
                throw new ArgumentNullException("compensateServiceBus");

            _log.InfoFormat("Creating Execute {0} Service Bus", _activityName);

            Uri compensateAddress = compensateServiceBus.Endpoint.Address.Uri;

            IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    _transportFactory.Configure(x, _executeQueueName, _executeConsumerLimit);
                    x.Subscribe(
                        s => s.ExecuteActivityHost<TActivity, TArguments>(compensateAddress, _executeActivityFactory));
                });

            return bus;
        }

        protected virtual IServiceBus CreateCompensateServiceBus()
        {
            _log.InfoFormat("Creating Compensate {0} Service Bus", _activityName);

            IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    _transportFactory.Configure(x, _compensateQueueName, _compensateConsumerLimit);
                    x.Subscribe(s => s.CompensateActivityHost<TActivity, TLog>(_compensateActivityFactory));
                });

            return bus;
        }
    }
}