namespace RapidTransit.Integration.Configuration
{
    using Autofac;
    using RapidTransit.Core.Configuration;
    using RapidTransit.Core.Mapping;
    using RapidTransit.Core.Reflection;


    public class ConfigurationProviderModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileConfigurationProvider>()
                   .As<IConfigurationProvider>()
                   .SingleInstance();

            builder.RegisterType<ConnectionStringProvider>()
                   .As<IConnectionStringProvider>()
                   .SingleInstance();

            builder.RegisterType<DynamicImplementationBuilder>()
                   .As<IImplementationBuilder>()
                   .SingleInstance();

            builder.RegisterType<DynamicObjectConverterCache>()
                   .As<IObjectConverterCache>()
                   .SingleInstance();

            builder.RegisterType<ConfigurationSettingsProvider>()
                   .As<ISettingsProvider>()
                   .SingleInstance();
        }
    }
}