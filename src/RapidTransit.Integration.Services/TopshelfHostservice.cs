namespace RapidTransit.Integration.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Logging;
    using Topshelf;


    public class TopshelfHostService :
        ServiceControl
    {
        readonly ILog _log = Logger.Get<TopshelfHostService>();
        readonly IEnumerable<IServiceBootstrapper> _serviceBootstrappers;

        IList<ServiceControl> _services;

        public TopshelfHostService(IEnumerable<IServiceBootstrapper> serviceBootstrappers)
        {
            _serviceBootstrappers = serviceBootstrappers;
        }

        public bool Start(HostControl hostControl)
        {
            _log.InfoFormat("Starting Subscriber Activity Service");

            var started = new List<ServiceControl>();

            try
            {
                _services = _serviceBootstrappers.Select(x => x.CreateService()).ToList();

                _log.InfoFormat("Starting {0} services", _services.Count());

                foreach (ServiceControl activityService in _services)
                {
                    hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                    StartService(hostControl, activityService);

                    started.Add(activityService);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Service failed to start", ex);

                Parallel.ForEach(started, service =>
                    {
                        hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                        StopService(hostControl, service);
                    });

                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _log.InfoFormat("Stopping {0} services", _services.Count());

            if (_services != null)
            {
                Parallel.ForEach(_services, service =>
                    {
                        hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                        StopService(hostControl, service);
                    });
            }

            return true;
        }

        void StartService(HostControl hostControl, ServiceControl service)
        {
            if (hostControl == null)
                throw new ArgumentNullException("hostControl");

            if (service == null)
                return;

            _log.InfoFormat("Starting Service {0}", service);

            if (!service.Start(hostControl))
                throw new TopshelfException(string.Format((string)"Failed to start service: {0}", (object)service));
        }

        void StopService(HostControl hostControl, ServiceControl service)
        {
            if (hostControl == null)
                throw new ArgumentNullException("hostControl");

            if (service == null)
                return;

            try
            {
                _log.InfoFormat("Stopping Service {0}", service);

                if (!service.Stop(hostControl))
                    throw new TopshelfException(string.Format((string)"Failed to stop service: {0}", (object)service));
            }
            catch (Exception ex)
            {
                _log.Error("Stop Service Failed", ex);
            }
        }
    }
}