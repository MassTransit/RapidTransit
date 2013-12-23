namespace RapidTransit.Integration.Services
{
    using Autofac;
    using Topshelf;


    public delegate ServiceControl ScopedServiceFactory(ILifetimeScope lifetimeScope);
}