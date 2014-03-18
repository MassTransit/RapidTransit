namespace RapidTransit.Integration.Services
{
    using Autofac;
    using MassTransit.Courier;
    using MassTransit.Logging;


    public class AutofacExecuteActivityFactory<TActivity, TArguments> :
        ExecuteActivityFactory<TArguments>
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<AutofacExecuteActivityFactory<TActivity, TArguments>>();
        readonly ILifetimeScope _lifetimeScope;

        public AutofacExecuteActivityFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ExecutionResult ExecuteActivity(Execution<TArguments> execution)
        {
            using (ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope(x => x.RegisterInstance(execution.Bus).ExternallyOwned()))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("ExecuteActivityFactory: Executing: {0}", typeof(TActivity).Name);

                var activity = scope.Resolve<TActivity>(TypedParameter.From(execution.Arguments));

                return activity.Execute(execution);
            }
        }
    }
}