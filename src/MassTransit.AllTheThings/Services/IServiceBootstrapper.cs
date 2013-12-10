namespace RapidTransit.Integration.Services
{
    using Topshelf;


    public interface IServiceBootstrapper
    {
        ServiceControl CreateService();
    }
}