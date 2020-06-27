using System;
using System.Threading.Tasks;
using MongoActor.DataAccess;
using NServiceBus;
using NServiceBus.Logging;
using TodoApp.Models;

namespace MongoActor
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            const string endpointName = "MongoActor";
            Console.Title = endpointName;

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(
                customizations: immediate =>
                {
                    immediate.NumberOfRetries(0);
                });
            recoverability.Delayed(
                customizations: delayed =>
                {
                    delayed.NumberOfRetries(2);
                });

            var endpointInstance = await Endpoint
                .Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance
                .Stop()
                .ConfigureAwait(false);
        }
    }
}
