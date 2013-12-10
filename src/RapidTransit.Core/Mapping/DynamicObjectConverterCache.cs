namespace RapidTransit.Core.Mapping
{
    using System;
    using RapidTransit.Core.Caching;
    using Reflection;


    /// <summary>
    /// Caches dictionary to object converters for the types requested, including the implementation
    /// builder for interfaces that are dynamically proxied
    /// </summary>
    public class DynamicObjectConverterCache :
        IObjectConverterCache
    {
        readonly ICache<Type, IObjectConverter> _cache;
        readonly IImplementationBuilder _implementationBuilder;

        public DynamicObjectConverterCache(IImplementationBuilder implementationBuilder)
        {
            _implementationBuilder = implementationBuilder;
            _cache = new ConcurrentCache<Type, IObjectConverter>(CreateMissingConverter);
        }

        public IObjectConverter GetConverter(Type type)
        {
            return _cache[type];
        }

        IObjectConverter CreateMissingConverter(Type type)
        {
            Type implementationType = _implementationBuilder.GetImplementationType(type);
            Type converterType = typeof(DynamicObjectConverter<,>).MakeGenericType(type, implementationType);

            return (IObjectConverter)Activator.CreateInstance(converterType, this);
        }
    }
}