using System;
using System.Data.SqlClient;
using NServiceBus;

namespace Shipping
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            const string endpointName = "Shipping";

            Console.Title = endpointName;

            var endpointConfiguration = ConfigureEndpoint(endpointName);

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance
                .Stop()
                .ConfigureAwait(false);
        }

        private static EndpointConfiguration ConfigureEndpoint(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            //ConfigureSerialization(endpointConfiguration);
            //ConfigureTransport(endpointConfiguration);
            ConfigurePersistence(endpointConfiguration);
            return endpointConfiguration;
        }

        private static void ConfigurePersistence(EndpointConfiguration endpointConfiguration)
        {
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(connection);
                });
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));
        }
        private static void ConfigureSerialization(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            //endpointConfiguration.AddDeserializer<NewtonsoftSerializer>();
        }
        private static void ConfigureTransport(EndpointConfiguration endpointConfiguration)
        {
            //endpointConfiguration.UseTransport<LearningTransport>();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            endpointConfiguration.EnableInstallers();
            transport.ConnectionString("host=localhost;username=guest;password=guest");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.AuditProcessedMessagesTo("personalQueueForAudit");
        }
    }
}
