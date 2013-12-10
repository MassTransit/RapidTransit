namespace RapidTransit.Integration.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Services;
    using MassTransit.Logging;
    using Topshelf;


    public class ServiceBusHostService :
        ServiceControl,
        IDisposable
    {
        readonly IServiceBusHost[] _hosts;
        readonly string _serviceName;
        readonly ITransportConfigurator _transportConfigurator;
        bool _disposed;

        public ServiceBusHostService(ITransportConfigurator transportConfigurator, IEnumerable<IServiceBusHost> hosts,
            string serviceName)
        {
            _transportConfigurator = transportConfigurator;
            _serviceName = serviceName;
            _hosts = hosts.ToArray();
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            Parallel.ForEach(_hosts, x => x.Dispose());

            _disposed = true;
        }

        public bool Start(HostControl hostControl)
        {
            OnStarting(hostControl);

            Logger.Get(GetType())
                  .InfoFormat("Creating {0} Service Buses for hosted service: {1}", _hosts.Length, _serviceName);

            try
            {
                Parallel.ForEach(_hosts, hostedServiceBus => { hostedServiceBus.Start(_transportConfigurator); });

                OnStarted(hostControl);

                Logger.Get(GetType())
                      .InfoFormat("Created {0} Service Buses for hosted service: {1}", _hosts.Length, _serviceName);

                return true;
            }
            catch (Exception ex)
            {
                Parallel.ForEach(_hosts, hostedServiceBus => hostedServiceBus.Dispose());

                OnStartFailed(hostControl, ex);
                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            OnStopping(hostControl);

            Logger.Get(GetType())
                  .InfoFormat("Stopping {0} Service Buses for hosted service: {1}", _hosts.Length, _serviceName);

            try
            {
                Parallel.ForEach(_hosts, hostedServiceBus => hostedServiceBus.Dispose());

                _disposed = true;

                OnStopped(hostControl);

                Logger.Get(GetType())
                      .InfoFormat("Stopped {0} Service Buses for hosted service: {1}", _hosts.Length, _serviceName);
            }
            catch (Exception ex)
            {
                OnStopFailed(hostControl, ex);
                throw;
            }

            return true;
        }

        protected virtual void OnStarting(HostControl hostControl)
        {
        }

        protected virtual void OnStarted(HostControl hostControl)
        {
        }

        protected virtual void OnStartFailed(HostControl hostControl, Exception exception)
        {
        }

        protected virtual void OnStopping(HostControl hostControl)
        {
        }

        protected virtual void OnStopped(HostControl hostControl)
        {
        }

        protected virtual void OnStopFailed(HostControl hostControl, Exception exception)
        {
        }
    }
}