using System;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    public class DistHandler : IHandleMessages<DistributedTransactionStarted>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();
        public Task Handle(DistributedTransactionStarted message, IMessageHandlerContext context)
        {
            log.Info($"Here I am Starting: {message.OrderId}");
            return Task.CompletedTask;
        }
    }
}
