namespace RapidTransit.Integration.Services
{
    using System;
    using Autofac;
    using Core.Services;
    using Topshelf;


    public abstract class ServiceBusInstanceServiceBootstrapper :
        IServiceBootstrapper
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;

        protected ServiceBusInstanceServiceBootstrapper(ILifetimeScope lifetimeScope, Type bootstrapperType)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = bootstrapperType.GetServiceDescription();
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceBusInstanceService>()
                   .SingleInstance()
                   .WithParameter(TypedParameter.From(_serviceName))
                   .As<ServiceControl>();
        }
    }
}