namespace RapidTransit.Integration.Configuration
{
    using Autofac;
    using Core;


    public class ManagementBusModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ManagementBusConfigurationSettings>()
                   .As<ManagementBusSettings>()
                   .SingleInstance();

            builder.RegisterType<ManagementBus>()
                   .As<IManagementBus>()
                   .SingleInstance();
        }
    }
}