using System;
using System.Linq;
using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
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

        private static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            var lastOrder = string.Empty;
            var duplicateId = Guid.NewGuid().ToString();

            while (true)
            {
                log.Info("Press 'P' to place an order, 'C' to cancel last order, 'W' to see deduplication, or 'Q' to quit.");
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

                    case ConsoleKey.W:
                        // Instantiate the command
                        var dupeEvent = new DuplicatesToPrevent();
                        await SendDuplicates(endpointInstance, dupeEvent, 5);
                        break;

                    case ConsoleKey.B:
                        // Send Business Event
                        var bizEvent = new BusinessEvent();
                        await SendDuplicates(endpointInstance, bizEvent, 5);
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }

        public static Task SendDuplicates<TMessage>(IMessageSession context, TMessage message, int totalCount)
        {
            var duplicatedMessageId = Guid.NewGuid().ToString();

            var tasks = Enumerable.Range(0, totalCount)
                .Select(i =>
                {
                    var options = new PublishOptions();
                    options.SetMessageId(duplicatedMessageId);
                    log.Info($"Sending Dupe Event with MessageId= {duplicatedMessageId}");
                    return context.Publish(message, options);
                });

            return Task.WhenAll(tasks);
        }
    }
}
