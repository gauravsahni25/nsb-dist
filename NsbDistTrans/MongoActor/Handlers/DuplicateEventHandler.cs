using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace MongoActor.Handlers
{
    class DuplicateEventHandler : IHandleMessages<DuplicatesToPrevent>
    {
        static ILog log = LogManager.GetLogger<DuplicateEventHandler>();

        public Task Handle(DuplicatesToPrevent message, IMessageHandlerContext context)
        {
            log.Info($"Received DuplicatesToPrevent, EventId = {context.MessageId}, should not be more than 1");
            return Task.CompletedTask;
        }
    }
}
