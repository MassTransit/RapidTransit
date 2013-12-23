namespace RapidTransit.Integration.Services.QuartzIntegration
{
    using System.Collections.Specialized;
    using Autofac;
    using Core.Configuration;
    using Core.Services;
    using MassTransit.Exceptions;
    using MassTransit.QuartzIntegration;
    using Quartz;
    using Quartz.Impl;


    public class MessageSchedulingServiceBootstrapper :
        ServiceBusHostServiceBootstrapper
    {
        public MessageSchedulingServiceBootstrapper(ILifetimeScope lifetimeScope)
            : base(lifetimeScope, typeof(MessageSchedulingServiceBootstrapper))
        {
        }

        protected override void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterAutofacConsumerFactory();

            builder.RegisterType<ScheduleMessageConsumer>()
                   .InstancePerMessageScope();

            builder.RegisterType<CancelScheduledMessageConsumer>()
                   .InstancePerMessageScope();

            builder.Register(CreateScheduler)
                   .SingleInstance()
                   .As<IScheduler>();

            builder.RegisterType<MessageSchedulingConsumerServiceBusHost>()
                   .As<IServiceBusHost>();

            base.ConfigureLifetimeScope(builder);
        }

        IScheduler CreateScheduler(IComponentContext context)
        {
            var configurationProvider = context.Resolve<IConfigurationProvider>();

            NameValueCollection quartzSettings;
            if (!configurationProvider.TryGetNameValueCollectionSection("quartz", out quartzSettings))
                throw new ConfigurationException("Could not load the configuration section");

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(quartzSettings);
            IScheduler scheduler = schedulerFactory.GetScheduler();

            return scheduler;
        }
    }
}