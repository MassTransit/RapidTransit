namespace RapidTransit.Integration.Services.QuartzIntegration
{
    using System;
    using Core.Configuration;
    using Core.Services;
    using MassTransit;
    using MassTransit.QuartzIntegration;
    using Quartz;


    public class MessageSchedulingConsumerServiceBusInstance :
        ServiceBusInstance
    {
        readonly IScheduler _scheduler;

        public MessageSchedulingConsumerServiceBusInstance(IConfigurationProvider configurationProvider,
            IConsumerFactory<ScheduleMessageConsumer> scheduleMessageConsumerFactory,
            IConsumerFactory<CancelScheduledMessageConsumer> cancelScheduledMessageConsumer,
            IScheduler scheduler)
            : base(configurationProvider, "MessageSchedulingQueueName", "MessageSchedulingConsumerLimit", 1)
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