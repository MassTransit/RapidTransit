namespace RapidTransit.Tests.Configuration
{
    using NUnit.Framework;
    using RapidTransit.Core.Configuration;


    [TestFixture]
    public class Using_the_connection_string_provider
    {
        [Test]
        public void Should_get_the_default()
        {
            IConfigurationProvider configurationProvider = new FileConfigurationProvider();
            IConnectionStringProvider connectionStringProvider = new ConnectionStringProvider(configurationProvider);

            string providerName;
            string connectionString;

            Assert.IsTrue(connectionStringProvider.TryGetConnectionString("default", out connectionString,
                out providerName));

            Assert.AreEqual("Server=localhost;Database=test", connectionString);
            Assert.AreEqual("easyProvider", providerName);
        }

        [Test]
        public void Should_get_the_default_with_server()
        {
            IConfigurationProvider configurationProvider = new FileConfigurationProvider();
            IConnectionStringProvider connectionStringProvider = new ConnectionStringProvider(configurationProvider);

            string providerName;
            string connectionString;

            Assert.IsTrue(connectionStringProvider.TryGetConnectionString("default", out connectionString,
                out providerName, "remote"));

            Assert.AreEqual("Server=remote;Database=test", connectionString);
            Assert.AreEqual("easyProvider", providerName);
        }
    }
}