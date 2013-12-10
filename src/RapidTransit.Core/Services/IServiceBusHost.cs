namespace RapidTransit.Core.Services
{
    using System;
    using MassTransit;


    public interface IServiceBusHost :
        IDisposable
    {
        IServiceBus Start(ITransportConfigurator transportConfigurator);
    }
}