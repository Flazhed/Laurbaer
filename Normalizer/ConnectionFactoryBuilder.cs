using RabbitMQ.Client;
namespace Normalizer
{
    class ConnectionFactoryBuilder
    {
            public static ConnectionFactory CreateConnectionFactory()
            {
                var factory = new ConnectionFactory() { HostName = Constants.HOSTNAME, UserName = Constants.USERNAME, Password = Constants.PASSWORD };
                return factory;
            }
    }
}
