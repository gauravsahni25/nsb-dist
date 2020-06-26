using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Billing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpointName = "Billing";

            Console.Title = endpointName;

            var endpointInstance = await Endpoint
                                .Start(new EndpointConfiguration(endpointName))
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
            return endpointConfiguration;
        }

        private static void ConfigureSerialization(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            //endpointConfiguration.AddDeserializer<XmlSerializer>();
        }

        private static void ConfigureTransport(EndpointConfiguration endpointConfiguration)
        {
            //var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            endpointConfiguration.EnableInstallers();
            transport.ConnectionString("host=localhost;username=guest;password=guest");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.AuditProcessedMessagesTo("personalQueueForAudit");
        }
    }
}

// Old Code
//var endpointConfiguration = ConfigureEndpoint(endpointName);
//var endpointInstance = await Endpoint.Start(endpointConfiguration)
//    .ConfigureAwait(false);