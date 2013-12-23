namespace RapidTransit.Integration.Web
{
    using Autofac;
    using Core;


    public class WebServiceBusModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WebServiceBusConfigurationSettings>()
                   .As<HostServiceBusSettings>()
                   .SingleInstance();

            builder.RegisterType<HostServiceBus>()
                   .As<IHostServiceBus>()
                   .SingleInstance();
        }
    }
}