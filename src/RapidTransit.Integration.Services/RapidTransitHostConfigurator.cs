namespace RapidTransit.Integration.Services
{
    using System;
    using System.Diagnostics;
    using Core.Services;
    using MassTransit.Monitoring;
    using Topshelf;
    using Topshelf.HostConfigurators;
    using Topshelf.Runtime;


    public class RapidTransitHostConfigurator<T>
        where T : TopshelfServiceBootstrapper<T>
    {
        T _bootstrapper;

        public RapidTransitHostConfigurator()
        {
            ServiceName = typeof(T).GetDisplayName();
            DisplayName = typeof(T).GetDisplayName();
            Description = typeof(T).GetServiceDescription();

            BootstrapperFactory = settings => (T)Activator.CreateInstance(typeof(T), settings);
        }

        public string ServiceName { private get; set; }
        public string DisplayName { private get; set; }
        public string Description { private get; set; }
        public Func<HostSettings, T> BootstrapperFactory { private get; set; }

        public void Configure(HostConfigurator configurator)
        {
            configurator.SetServiceName(ServiceName);
            configurator.SetDisplayName(DisplayName);
            configurator.SetDescription(Description);

            configurator.AfterInstall(() =>
                {
                    VerifyEventLogSourceExists(ServiceName);

                    // this will force the performance counters to register during service installation
                    // making them created - of course using the InstallUtil stuff completely skips
                    // this part of the install :(
                    ServiceBusPerformanceCounters counters = ServiceBusPerformanceCounters.Instance;
                });

            configurator.Service(settings =>
                {
                    _bootstrapper = BootstrapperFactory(settings);

                    return _bootstrapper.GetService();
                },
                s => s.AfterStoppingService(() =>
                    {
                        if (_bootstrapper != default(T))
                            _bootstrapper.Dispose();
                    }));
        }

        static void VerifyEventLogSourceExists(string serviceName)
        {
            if (!EventLog.SourceExists(serviceName))
                EventLog.CreateEventSource(serviceName, "RelayHealth");
        }

        public event OnHostStarting OnStarting;
    }
}