namespace RapidTransit.Integration.Configuration
{
    using System.Configuration;
    using Autofac;
    using Core;
    using Core.Configuration;


    public class RabbitMqConfigurationModule :
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

            builder.Register(GetRabbitMqSettings)
                   .As<RabbitMqSettings>()
                   .SingleInstance();

            builder.RegisterType<RabbitMqTransportConfigurator>()
                   .As<ITransportConfigurator>();
        }

        static RabbitMqSettings GetRabbitMqSettings(IComponentContext context)
        {
            RabbitMqSettings settings;
            if (context.Resolve<ISettingsProvider>().TryGetSettings("RabbitMQ", out settings))
                return settings;

            throw new ConfigurationErrorsException("Unable to resolve RabbitMqSettings from configuration");
        }
    }
}