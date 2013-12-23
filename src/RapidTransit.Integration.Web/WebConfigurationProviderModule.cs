namespace RapidTransit.Integration.Web
{
    using Autofac;
    using Core.Configuration;


    public class WebConfigurationProviderModule :
        ConfigurationProviderModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WebConfigurationProvider>()
                   .WithParameter(new NamedParameter("configurationPath", "/"))
                   .As<IConfigurationProvider>()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}