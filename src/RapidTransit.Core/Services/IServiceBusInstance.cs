namespace RapidTransit.Core.Services
{
    using System;
    using MassTransit;


    public interface IServiceBusInstance :
        IDisposable
    {
        IServiceBus Start(ITransportConfigurator transportConfigurator);
    }
}