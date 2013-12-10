namespace RapidTransit.Core
{
    using Automatonymous;


    public interface IStateMachineActivityFactory
    {
        Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>()
            where TActivity : Activity<TInstance, TData>;

        Activity<TInstance> GetActivity<TActivity, TInstance>()
            where TActivity : Activity<TInstance>;
    }
}