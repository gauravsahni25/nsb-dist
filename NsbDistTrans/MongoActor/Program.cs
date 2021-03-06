﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace MongoActor
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            const string endpointName = "MongoActor";
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
            endpointConfiguration.EnableOutbox();
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
    }
}
