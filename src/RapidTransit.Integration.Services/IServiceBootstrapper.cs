namespace RapidTransit.Integration.Services
{
    using Topshelf;


    public interface IServiceBootstrapper
    {
        string LifetimeScopeTag { get; }
        string ServiceName { get; }
        ServiceControl CreateService();
    }
}