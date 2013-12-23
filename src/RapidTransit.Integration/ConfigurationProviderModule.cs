namespace RapidTransit.Integration
{
    using Autofac;
    using Core.Configuration;
    using Core.Mapping;
    using Core.Reflection;


    public abstract class ConfigurationProviderModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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