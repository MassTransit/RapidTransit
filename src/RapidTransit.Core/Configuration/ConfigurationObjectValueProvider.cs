namespace RapidTransit.Core.Configuration
{
    using Mapping;


    public class ConfigurationObjectValueProvider :
        IObjectValueProvider
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly string _prefix;

        public ConfigurationObjectValueProvider(IConfigurationProvider configurationProvider, string prefix = null)
        {
            _configurationProvider = configurationProvider;
            _prefix = prefix ?? "";
        }

        public bool TryGetValue(string name, out object value)
        {
            string settingValue;
            bool found = _configurationProvider.TryGetSetting(_prefix + name, out settingValue);

            value = settingValue;
            return found;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            object obj;
            if (TryGetValue(name, out obj))
            {
                if (obj is T)
                {
                    value = (T)obj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}