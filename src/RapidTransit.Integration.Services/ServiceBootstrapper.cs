namespace RapidTransit.Integration.Services
{
    using Autofac;
    using Core.Services;
    using Topshelf;


    public abstract class ServiceBootstrapper<TService> :
        IServiceBootstrapper
        where TService : ServiceControl
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;

        protected ServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = typeof(TService).GetServiceDescription();
        }


        protected ServiceBootstrapper(ILifetimeScope lifetimeScope, string serviceName)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = serviceName;
        }


        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TService>()
                .SingleInstance()
                .As<ServiceControl>()
                .As<TService>();
        }
    }
}