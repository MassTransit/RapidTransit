namespace RapidTransit.Integration
{
    using Autofac;
    using Automatonymous;
    using Core;


    public class AutofacStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        readonly IComponentContext _context;

        public AutofacStateMachineActivityFactory(IComponentContext context)
        {
            _context = context;
        }

        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>()
            where TActivity : Activity<TInstance, TData>
        {
            return _context.Resolve<TActivity>();
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>()
            where TActivity : Activity<TInstance>
        {
            return _context.Resolve<TActivity>();
        }
    }
}