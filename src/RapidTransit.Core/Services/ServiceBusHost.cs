namespace RapidTransit.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Configuration;
    using MassTransit;
    using MassTransit.Configurators;
    using MassTransit.Logging;
    using MassTransit.SubscriptionConfigurators;


    /// <summary>
    /// A hosted service bus allows the configuration of a service bus to be contained in a class
    /// making it easier to manage service dependencies
    /// </summary>
    public abstract class ServiceBusHost :
        SubscriptionBusServiceConfigurator,
        IServiceBusHost
    {
        static readonly ILog _log = Logger.Get<ServiceBusHost>();
        readonly IList<SubscriptionBusServiceBuilderConfigurator> _configurators;
        readonly int _consumerLimit;
        readonly string _queueName;
        IServiceBus _bus;
        bool _disposed;


        protected ServiceBusHost(IConfigurationProvider configuration, string queueKey, string consumerLimitKey,
            int defaultConsumerLimit)
        {
            string queueName;
            if (!configuration.TryGetSetting(queueKey, out queueName))
                throw new ConfigurationErrorsException("Unable to load queue name from key: " + queueKey);

            _queueName = queueName;
            _consumerLimit = configuration.GetSetting(consumerLimitKey, defaultConsumerLimit);
            _configurators = new List<SubscriptionBusServiceBuilderConfigurator>();
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;

            if (_bus != null)
            {
                _log.InfoFormat("{0} Stopping Service Bus: {1}", GetType().GetFriendlyDescription(), _bus.Endpoint.Address.Uri);

                OnDisposing(_bus);

                try
                {
                    _bus.Dispose();
                    _bus = null;

                    OnDisposed();
                }
                catch (Exception ex)
                {
                    OnDisposeFailed(ex);
                    throw;
                }
            }

            _disposed = true;
        }

        public IServiceBus Start(ITransportConfigurator transportConfigurator)
        {
            OnStarting();

            try
            {
                _log.InfoFormat("{0} Starting Service Bus for Queue: {1}({2})", GetType().GetFriendlyDescription(), _queueName, _consumerLimit);
                
                IServiceBus bus = ServiceBusFactory.New(x =>
                    {
                        transportConfigurator.Configure(x, _queueName, _consumerLimit);

                        x.Subscribe(s =>
                            {
                                foreach (SubscriptionBusServiceBuilderConfigurator builderConfigurator in _configurators)
                                    s.AddConfigurator(builderConfigurator);
                            });
                    });

                OnStarted(bus);

                _bus = bus;

                return bus;
            }
            catch (Exception ex)
            {
                OnStartFailed(ex);
                throw;
            }
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            throw new NotImplementedException("Nothing should validate this as it is just a configuration wrapper");
        }

        void SubscriptionBusServiceConfigurator.AddConfigurator(SubscriptionBusServiceBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        protected virtual void OnStarting()
        {
        }

        protected virtual void OnStarted(IServiceBus bus)
        {
        }

        protected virtual void OnStartFailed(Exception exception)
        {
        }

        protected virtual void OnDisposing(IServiceBus bus)
        {
        }

        protected virtual void OnDisposed()
        {
        }

        protected virtual void OnDisposeFailed(Exception exception)
        {
        }
    }
}