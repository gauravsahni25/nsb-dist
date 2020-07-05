using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Messages.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace MongoActor.Handlers
{
    class BadStuffHandler : IHandleMessages<RequestCommand>, IHandleMessages<RequestEvent>, IHandleMessages<RequestMessage>
    {
        static ILog log = LogManager.GetLogger<BadStuffHandler>();
        public Task Handle(RequestCommand message, IMessageHandlerContext context)
        {
            var response = new ResponseCommand()
            {
                DataId = 3,
                Message = "This is the command response"
            };
            log.Info($"Processing your request with Attributes: {message.Message}");
            return context.Reply(response);
        }

        public Task Handle(RequestEvent message, IMessageHandlerContext context)
        {
            var response = new ResponseEvent()
            {
                DataId = 4,
                Message = "This is the event response"
            };
            log.Info($"Processing your request with Attributes: {message.Message}");
            return context.Reply(response);
        }

        public Task Handle(RequestMessage message, IMessageHandlerContext context)
        {
            var response = new ResponseMessage()
            {
                DataId = 4,
                Message = "This is the Message response"
            };
            log.Info($"Processing your request with Attributes: {message.Message}");
            return context.Reply(response);
        }
    }
}
