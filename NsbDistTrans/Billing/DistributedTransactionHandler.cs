using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Billing
{
    public class DistributedTransactionHandler : IHandleMessages<BusinessEvent>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();

        public Task Handle(BusinessEvent message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.EventId} - Charging credit card...");
            return Task.CompletedTask;
        }
    }
}
