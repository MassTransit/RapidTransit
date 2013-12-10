namespace RapidTransit.Core.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using RapidTransit.Core.Caching;


    public class InterfaceReflectionCache
    {
        readonly ICache<Type, ICache<Type, Type>> _cache;

        public InterfaceReflectionCache()
        {
            _cache = new ConcurrentCache<Type, ICache<Type, Type>>(typeKey =>
                {
                    MissingValueProvider<Type, Type> missingValueProvider = x => GetInterfaceInternal(typeKey, x);

                    return new ConcurrentCache<Type, Type>(missingValueProvider);
                });
        }

        Type GetInterfaceInternal(Type type, Type interfaceType)
        {
            if (interfaceType.GetTypeInfo().IsGenericTypeDefinition)
                return GetGenericInterface(type, interfaceType);

            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i] == interfaceType)
                    return interfaces[i];
            }

            return null;
        }

        public Type GetGenericInterface(Type type, Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    "The interface must be a generic interface definition: " + interfaceType.Name,
                    "interfaceType");
            }

            // our contract states that we will not return generic interface definitions without generic type arguments
            if (type == interfaceType)
                return null;
            if (type.GetTypeInfo().IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == interfaceType)
                    return type;
            }
            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].GetTypeInfo().IsGenericType)
                {
                    if (interfaces[i].GetGenericTypeDefinition() == interfaceType)
                        return interfaces[i];
                }
            }

            return null;
        }

        public Type Get(Type type, Type interfaceType)
        {
            return _cache[type][interfaceType];
        }
    }
}