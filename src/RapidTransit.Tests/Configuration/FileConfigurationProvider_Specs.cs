namespace RapidTransit.Tests.Configuration
{
    using NUnit.Framework;
    using RapidTransit.Core.Configuration;


    [TestFixture]
    public class Using_the_file_configuration_provider
    {
        [Test]
        public void Should_load_an_existing_int_value()
        {
            IConfigurationProvider configurationProvider = new FileConfigurationProvider();

            int intValue = configurationProvider.GetSetting("SimpleValue", 0);

            Assert.AreEqual(42, intValue);
        }

        [Test]
        public void Should_load_an_existing_string_value()
        {
            IConfigurationProvider configurationProvider = new FileConfigurationProvider();

            string stringValue;
            bool present = configurationProvider.TryGetSetting("SimpleString", out stringValue);

            Assert.IsTrue(present);
            Assert.AreEqual("Hello, World.", stringValue);
        }
    }
}