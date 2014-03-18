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


    /// <summary>
    /// For an activity that has no compensation, only create the execute portion of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public class ExecuteActivityService<TActivity, TArguments> :
        ServiceControl,
        IDisposable
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly string _activityName;
        readonly IActivityUriProvider _activityUriProvider;
        readonly ExecuteActivityFactory<TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;
        readonly ILog _log;
        readonly ITransportConfigurator _transportFactory;
        bool _disposed;
        IServiceBus _executeBus;

        public ExecuteActivityService(IConfigurationProvider configuration, ITransportConfigurator transportFactory,
            IActivityUriProvider activityUriProvider, ExecuteActivityFactory<TArguments> executeActivityFactory)
        {
            _log = Logger.Get(GetType());

            _transportFactory = transportFactory;
            _activityUriProvider = activityUriProvider;
            _executeActivityFactory = executeActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = _activityUriProvider.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            if (_executeBus != null)
                _executeBus.Dispose();

            _disposed = true;
        }

        public virtual bool Start(HostControl hostControl)
        {
            _executeBus = CreateExecuteServiceBus();

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

        protected virtual IServiceBus CreateExecuteServiceBus()
        {
            _log.InfoFormat("Creating Execute {0} Service Bus", _activityName);

            // TODO remove invalid address once courier gets a clue
            var compensateAddress = new Uri("error://unknown");

            IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    _transportFactory.Configure(x, _executeQueueName, _executeConsumerLimit);
                    x.Subscribe(
                        s => s.ExecuteActivityHost<TActivity, TArguments>(compensateAddress, _executeActivityFactory));
                });

            return bus;
        }
    }
}