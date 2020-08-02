using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;

namespace Sales
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpointName = "Sales";
            
            Console.Title = endpointName;

            var endpointConfiguration = ConfigureEndpoint(endpointName);
            var endpointInstance = await Endpoint
                                .Start(endpointConfiguration)
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
            ConfigurePersistence(endpointConfiguration);
            return endpointConfiguration;
        }

        private static void ConfigurePersistence(EndpointConfiguration endpointConfiguration)
        {
            var sqlServerPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
            sqlServerPersistence.SqlDialect<SqlDialect.MsSqlServer>();
            sqlServerPersistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(connection);
                });
            var subscriptions = sqlServerPersistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));


            // Enable Outbox
            var outboxSettings = endpointConfiguration.EnableOutbox();
            outboxSettings.UsePessimisticConcurrencyControl();
            outboxSettings.KeepDeduplicationDataFor(TimeSpan.FromDays(30));
        }
    }
}
