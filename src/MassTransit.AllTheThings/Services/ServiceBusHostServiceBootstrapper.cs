namespace RapidTransit.Integration.Services
{
    using System;
    using Autofac;
    using Core.Services;
    using Topshelf;


    public abstract class ServiceBusHostServiceBootstrapper :
        IServiceBootstrapper
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;

        protected ServiceBusHostServiceBootstrapper(ILifetimeScope lifetimeScope, Type bootstrapperType)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = bootstrapperType.GetFriendlyDescription();
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceBusHostService>()
                   .SingleInstance()
                   .WithParameter(TypedParameter.From(_serviceName))
                   .As<ServiceControl>();
        }
    }
}