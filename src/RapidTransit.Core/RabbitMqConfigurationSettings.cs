namespace RapidTransit.Core
{
    using System.Configuration;
    using Configuration;


    public class RabbitMqConfigurationSettings :
        RabbitMqSettings
    {
        public RabbitMqConfigurationSettings(IConfigurationProvider configurationProvider)
        {
            string host;
            if (false == configurationProvider.TryGetSetting("RabbitMQHost", out host))
                throw new ConfigurationErrorsException("The RabbitMQHost was not configured");

            string username;
            if (false == configurationProvider.TryGetSetting("RabbitMQUsername", out username))
                throw new ConfigurationErrorsException("The RabbitMQUsername was not configured");

            string password;
            if (false == configurationProvider.TryGetSetting("RabbitMQPassword", out password))
                throw new ConfigurationErrorsException("The RabbitMQPassword was not configured");

            Host = host;
            Username = username;
            Password = password;

            Heartbeat = (ushort)configurationProvider.GetSetting("RabbitMQHeartbeat", 30);
            Port = configurationProvider.GetSetting("RabbitMQPort", 5893);
            VirtualHost = configurationProvider.GetSetting("RabbitMQVirtualHost", "");
            Options = configurationProvider.GetSetting("RabbitMQOptions", "");
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public ushort Heartbeat { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string VirtualHost { get; private set; }
        public string Options { get; private set; }
    }
}