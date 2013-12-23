namespace RapidTransit.Core
{
    using System;
    using MassTransit;
    using MassTransit.Diagnostics.Introspection;
    using MassTransit.Logging;
    using MassTransit.Pipeline;


    /// <summary>
    /// The host service bus is a per-service bus instance that is created and added to the 
    /// root container. It can be used by any services for service-level event notifications, such as
    /// cache item expirations that need to be received by every service instance on every server running
    /// the service
    /// </summary>
    public class HostServiceBus :
        IHostServiceBus
    {
        readonly IServiceBus _bus;
        readonly ILog _log = Logger.Get<HostServiceBus>();
        bool _disposed;

        public HostServiceBus(ITransportConfigurator transportConfigurator, HostServiceBusSettings settings)
        {
            _log.DebugFormat("Creating host service bus on queue: {0}", settings.QueueName);

            IServiceBus bus = ServiceBusFactory.New(x => transportConfigurator.Configure(x, settings.QueueName, 1));
            _bus = bus;
            _log.DebugFormat("host service bus created: {0}", _bus.Endpoint.Address.Uri);
        }

        public void Dispose()
        {
            lock (_bus)
            {
                if (_disposed)
                    return;

                _disposed = true;
            }

            _log.DebugFormat("Disposing host service bus: {0}", _bus.Endpoint.Address.Uri);

            _bus.Dispose();
        }

        public void Inspect(DiagnosticsProbe probe)
        {
            _bus.Inspect(probe);
        }

        public void Publish<T>(T message) where T : class
        {
            _bus.Publish(message);
        }

        public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback) where T : class
        {
            _bus.Publish(message, contextCallback);
        }

        public void Publish(object message)
        {
            _bus.Publish(message);
        }

        public void Publish(object message, Type messageType)
        {
            _bus.Publish(message, messageType);
        }

        public void Publish(object message, Action<IPublishContext> contextCallback)
        {
            _bus.Publish(message, contextCallback);
        }

        public void Publish(object message, Type messageType, Action<IPublishContext> contextCallback)
        {
            _bus.Publish(message, messageType, contextCallback);
        }

        public void Publish<T>(object values) where T : class
        {
            _bus.Publish<T>(values);
        }

        public void Publish<T>(object values, Action<IPublishContext<T>> contextCallback) where T : class
        {
            _bus.Publish(values, contextCallback);
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            return _bus.GetEndpoint(address);
        }

        public UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure)
        {
            return _bus.Configure(configure);
        }

        public IBusService GetService(Type type)
        {
            return _bus.GetService(type);
        }

        public bool TryGetService(Type type, out IBusService result)
        {
            return _bus.TryGetService(type, out result);
        }

        public IEndpoint Endpoint
        {
            get { return _bus.Endpoint; }
        }

        public IInboundMessagePipeline InboundPipeline
        {
            get { return _bus.InboundPipeline; }
        }

        public IOutboundMessagePipeline OutboundPipeline
        {
            get { return _bus.OutboundPipeline; }
        }

        public IServiceBus ControlBus
        {
            get { return _bus.ControlBus; }
        }

        public IEndpointCache EndpointCache
        {
            get { return _bus.EndpointCache; }
        }

        public TimeSpan ShutdownTimeout
        {
            get { return _bus.ShutdownTimeout; }
            set { _bus.ShutdownTimeout = value; }
        }
    }
}