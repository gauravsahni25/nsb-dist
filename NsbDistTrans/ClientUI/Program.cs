using System;
using System.Threading.Tasks;
using Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            const string endpointName = "ClientUI";

            Console.Title = endpointName;
            
            var endpointInstance = await Endpoint
                                .Start(new EndpointConfiguration(endpointName))
                                .ConfigureAwait(false);

            await RunLoop(endpointInstance)
                .ConfigureAwait(false);

            
            await endpointInstance
                .Stop()
                .ConfigureAwait(false);

        }

        private static EndpointConfiguration ConfigureEndpointAndRouting(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            //ConfigureSerialization(endpointConfiguration);
            //ConfigureTransport(endpointConfiguration);
            return endpointConfiguration;
        }

        private static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            var lastOrder = string.Empty;

            while (true)
            {
                log.Info("Press 'P' to place an order, 'C' to cancel last order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new PlaceOrder
                        {
                            OrderId = Guid.NewGuid().ToString()
                        };

                        // Send the command
                        log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await endpointInstance.Send(command)
                            .ConfigureAwait(false);

                        lastOrder = command.OrderId; // Store order identifier to cancel if needed.
                        break;

                    case ConsoleKey.C:
                        var cancelCommand = new CancelOrder
                        {
                            OrderId = lastOrder
                        };
                        await endpointInstance.Send(cancelCommand)
                            .ConfigureAwait(false);
                        log.Info($"Sent a correlated message to Cancel: {cancelCommand.OrderId}");
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }

        private static void ConfigureTransport(EndpointConfiguration endpointConfiguration)
        {
            // var transport = endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.EnableInstallers();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.ConnectionString("host=localhost;username=guest;password=guest");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.AuditProcessedMessagesTo("personalQueueForAudit");

            // Command Routing
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");
            routing.RouteToEndpoint(typeof(CancelOrder), "Sales");
        }

        private static void ConfigureSerialization(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        }
    }
}
// Old Code
// var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);