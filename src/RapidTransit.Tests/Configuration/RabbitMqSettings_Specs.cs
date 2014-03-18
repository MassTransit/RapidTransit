namespace RapidTransit.Tests.Configuration
{
    using System.Collections.Generic;
    using Core;
    using Core.Configuration;
    using Core.Mapping;
    using Core.Reflection;
    using NUnit.Framework;


    [TestFixture]
    public class RabbitMqSettings_Specs
    {
        [Test]
        public void Should_map_an_anonymous_object_to_the_settings()
        {
            var converterCache = new DynamicObjectConverterCache(new DynamicImplementationBuilder());
            var cache = new DictionaryConverterCache();

            var source = new
                {
                    Host = "localhost",
                    VirtualHost = "vortex",
                    Username = "joe",
                    Password = "guess",
                    Port = 5672,
                };

            IDictionaryConverter converter = cache.GetConverter(source.GetType());
            IDictionary<string, object> dictionary = converter.GetDictionary(source);

            IObjectConverter objectConverter = converterCache.GetConverter(typeof(RabbitMqSettings));

            var settings = (RabbitMqSettings)objectConverter.GetObject(dictionary);

            Assert.IsNotNull(settings);

            Assert.AreEqual("localhost", settings.Host);
            Assert.AreEqual("vortex", settings.VirtualHost);
            Assert.AreEqual("joe", settings.Username);
            Assert.AreEqual("guess", settings.Password);
            Assert.AreEqual(5672, settings.Port);
            Assert.AreEqual(0, settings.Heartbeat);

            Assert.IsNull(settings.Options);
        }

        [Test]
        public void Should_map_the_settings_from_the_configuration_provider()
        {
            IConfigurationProvider configurationProvider = new FileConfigurationProvider();
            var converterCache = new DynamicObjectConverterCache(new DynamicImplementationBuilder());
            var settingsProvider = new ConfigurationSettingsProvider(configurationProvider, converterCache);

            RabbitMqSettings settings;
            Assert.IsTrue(settingsProvider.TryGetSettings("RabbitMQ", out settings));

            Assert.IsNotNull(settings);

            Assert.AreEqual("localhost", settings.Host);
            Assert.AreEqual("vortex", settings.VirtualHost);
            Assert.AreEqual("joe", settings.Username);
            Assert.AreEqual("guess", settings.Password);
            Assert.AreEqual(5672, settings.Port);
            Assert.AreEqual(0, settings.Heartbeat);

            Assert.IsNull(settings.Options);
        }
    }
}