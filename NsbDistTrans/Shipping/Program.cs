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
            ConfigurePersistence(endpointConfiguration);
            return endpointConfiguration;
        }

        private static void ConfigurePersistence(EndpointConfiguration endpointConfiguration)
        {
            var mongo = endpointConfiguration.UsePersistence<MongoPersistence>();


            // Enable Outbox
            endpointConfiguration.EnableOutbox();
            mongo.TimeToKeepOutboxDeduplicationData(TimeSpan.FromDays(30));
            // default is pessimistic locking
        }
    }
}
