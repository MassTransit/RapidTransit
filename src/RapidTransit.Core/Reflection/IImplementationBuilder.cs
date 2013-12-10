namespace RapidTransit.Core.Reflection
{
    using System;


    public interface IImplementationBuilder
    {
        Type GetImplementationType(Type interfaceType);
    }
}