namespace RapidTransit.Core
{
    using Configuration;


    public interface ManagementBusSettings :
        ISettings
    {
        string QueueName { get; }
    }
}