using System;
using System.Linq;
using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Messages.Messages;
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
                log.Info("\nPress 'P' to place an order \n'C' to cancel last order \n'W' to see de-duplication \n'S' for StorageOps \n'X'No Handler Command \n'R' forRequest Response \n'Q' to quit.");
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

                    case ConsoleKey.S:
                        // Instantiate the command
                        log.Info($"Sending StorageOps Command");
                        await endpointInstance.Publish(new StorageOps());
                        break;

                    case ConsoleKey.R:
                        log.Info($"Request Response Demo");
                        await DemoRequestReply(endpointInstance);
                        break;

                    case ConsoleKey.X:
                        log.Info($"Sending Handler-Less Event and Command");
                        // Instantiate the command
                        // await endpointInstance.Publish(new HandlerLessEvent());
                        await endpointInstance.Publish(new HandlerLessCommand());
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }

        private static async Task DemoRequestReply(IEndpointInstance endpointInstance)
        {
            log.Info("\nPress 'C' to demo Command \n'E' to demo Event \n'M' to send Message");
            var key = Console.ReadKey();
            Console.WriteLine();
            switch (key.Key)
            {
                case ConsoleKey.C:
                    // Instantiate the command
                    var command = new RequestCommand()
                    {
                        DataId = 1, Message = "Command"
                    };
                    log.Info($"Sending Request Command with Id: {command.DataId}");
                    await endpointInstance.Send(command);
                    break;
                case ConsoleKey.E:
                    // Instantiate the command
                    var eventMessage = new RequestEvent()
                    {
                        DataId = 2,
                        Message = "Event"
                    };
                    log.Info($"Sending Request Event with Id: {eventMessage.DataId}");
                    await endpointInstance.Publish(eventMessage);
                    break;
                case ConsoleKey.M:
                    // Instantiate the message
                    var request = new RequestMessage()
                    {
                        DataId = 2,
                        Message = "Message"
                    };
                    log.Info($"Sending Request Event with Id: {request.Message}");
                    await endpointInstance.Send(request);
                    break;
                default:
                    log.Info("Exiting...");
                    return;
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
