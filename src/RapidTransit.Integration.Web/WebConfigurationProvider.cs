namespace RapidTransit.Integration.Web
{
    using System;
    using System.Configuration;
    using System.Web.Configuration;
    using Core.Configuration;


    public class WebConfigurationProvider :
        ConfigurationProviderBase
    {
        readonly Func<AppSettingsSection> _appSettings;
        readonly Func<ConnectionStringsSection> _connectionStrings;
        readonly Func<string, ConfigurationSection> _getSection;


        public WebConfigurationProvider(string configurationPath)
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration(configurationPath);

            _appSettings = GetAppSettings(configuration);

            _connectionStrings = GetConnectionStrings(configuration);
            _getSection = configuration.GetSection;
        }

        protected override AppSettingsSection GetAppSettings()
        {
            return _appSettings();
        }

        protected override ConnectionStringsSection GetConnectionStrings()
        {
            return _connectionStrings();
        }

        protected override ConfigurationSection GetSection(string sectionName)
        {
            return _getSection(sectionName);
        }
    }
}