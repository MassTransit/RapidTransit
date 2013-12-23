namespace RapidTransit.Integration.Services
{
    using Autofac;
    using Core.Configuration;


    public class ServiceConfigurationProviderModule :
        ConfigurationProviderModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileConfigurationProvider>()
                   .As<IConfigurationProvider>()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}