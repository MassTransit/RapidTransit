namespace RapidTransit.Core
{
    using Configuration;


    public interface HostServiceBusSettings :
        ISettings
    {
        string QueueName { get; }
    }
}