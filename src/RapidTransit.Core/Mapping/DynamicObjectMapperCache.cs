namespace RapidTransit.Core.Mapping
{
    using System;
    using Reflection;


    public class DynamicObjectMapperCache :
        IObjectMapperCache
    {
        readonly IObjectConverterCache _dtoCache;
        readonly IImplementationBuilder _implementationBuilder;
        readonly DictionaryConverterCache _otdCache;

        public DynamicObjectMapperCache()
        {
            _implementationBuilder = new DynamicImplementationBuilder();
            _dtoCache = new DynamicObjectConverterCache(_implementationBuilder);
            _otdCache = new DictionaryConverterCache();
        }

        public IObjectConverter GetObjectConverter(Type type)
        {
            return _dtoCache.GetConverter(type);
        }

        public IDictionaryConverter GetDictionaryConverter(Type type)
        {
            return _otdCache.GetConverter(type);
        }
    }
}