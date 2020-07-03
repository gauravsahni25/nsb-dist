using System;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Billing
{
    class DuplicateEventHandler : IHandleMessages<DuplicatesToPrevent>
    {
        static ILog log = LogManager.GetLogger<DuplicateEventHandler>();

        public Task Handle(DuplicatesToPrevent message, IMessageHandlerContext context)
        {
            log.Info($"Received DuplicatesToPrevent, EventId = {context.MessageId}, should be a lot more dupes here ");
            return Task.CompletedTask;
        }
    }
}
