namespace RapidTransit.Integration.Services
{
    using Autofac;
    using MassTransit.Courier;


    public class AutofacCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TLog>
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacCompensateActivityFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public CompensationResult CompensateActivity(Compensation<TLog> compensation)
        {
            using (ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope(x => x.RegisterInstance(compensation.Bus).ExternallyOwned()))
            {
                var activity = scope.Resolve<TActivity>(TypedParameter.From(compensation.Log));

                return activity.Compensate(compensation);
            }
        }
    }
}