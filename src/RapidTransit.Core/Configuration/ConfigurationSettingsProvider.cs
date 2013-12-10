namespace RapidTransit.Core.Configuration
{
    using Mapping;


    public class ConfigurationSettingsProvider :
        ISettingsProvider
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly IObjectConverterCache _converterCache;

        public ConfigurationSettingsProvider(IConfigurationProvider configurationProvider, IObjectConverterCache convertCache)
        {
            _configurationProvider = configurationProvider;
            _converterCache = convertCache;
        }

        public bool TryGetSettings<T>(string prefix, out T settings)
            where T : ISettings
        {
            IObjectConverter converter = _converterCache.GetConverter(typeof(T));

            var provider = new ConfigurationObjectValueProvider(_configurationProvider, prefix);

            settings = (T)converter.GetObject(provider);
            return true;
        }

        public bool TryGetSettings<T>(out T settings)
            where T : ISettings
        {
            IObjectConverter converter = _converterCache.GetConverter(typeof(T));

            var provider = new ConfigurationObjectValueProvider(_configurationProvider);

            settings = (T)converter.GetObject(provider);
            return true;
        }
    }
}