namespace RapidTransit.Integration.Services
{
    using Autofac;
    using Core;


    public class HostServiceBusModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HostServiceBusConfigurationSettings>()
                   .As<HostServiceBusSettings>()
                   .SingleInstance();

            builder.RegisterType<HostServiceBus>()
                   .As<IHostServiceBus>()
                   .SingleInstance();
        }
    }
}