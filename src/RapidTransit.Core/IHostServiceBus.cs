﻿namespace RapidTransit.Core
{
    using MassTransit;


    /// <summary>
    /// A bus for each service instance to receive responses to commands and events.
    /// All subscriptions should be transient
    /// </summary>
    public interface IHostServiceBus :
        IServiceBus
    {
    }
}