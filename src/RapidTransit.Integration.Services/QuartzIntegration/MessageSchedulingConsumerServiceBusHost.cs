namespace RapidTransit.Integration.Services.QuartzIntegration
{
    using System;
    using Core.Configuration;
    using Core.Services;
    using MassTransit;
    using MassTransit.QuartzIntegration;
    using Quartz;


    public class MessageSchedulingConsumerServiceBusHost :
        ServiceBusHost
    {
        readonly IScheduler _scheduler;

        public MessageSchedulingConsumerServiceBusHost(IConfigurationProvider configuration,
            IConsumerFactory<ScheduleMessageConsumer> scheduleMessageConsumerFactory,
            IConsumerFactory<CancelScheduledMessageConsumer> cancelScheduledMessageConsumer,
            IScheduler scheduler)
            : base(configuration, "MessageSchedulingQueueName", "MessageSchedulingConsumerLimit", 1)
        {
            _scheduler = scheduler;

            this.Consumer(scheduleMessageConsumerFactory);
            this.Consumer(cancelScheduledMessageConsumer);
        }

        protected override void OnStarted(IServiceBus bus)
        {
            _scheduler.JobFactory = new MassTransitJobFactory(bus);

            _scheduler.Start();
        }

        protected override void OnStartFailed(Exception exception)
        {
            _scheduler.Shutdown();
        }

        protected override void OnDisposing(IServiceBus bus)
        {
            _scheduler.Shutdown();
        }
    }
}