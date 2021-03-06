﻿using System;
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
    }
}