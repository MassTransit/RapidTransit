namespace RapidTransit.Core
{
    using System;


    public interface IActivityUriProvider
    {
        Uri GetExecuteActivityUri(string activityName);
        Uri GetCompensateActivityUri(string activityName);
        string GetExecuteActivityQueueName(string activityName);
        string GetCompensateActivityQueueName(string activityName);
    }
}