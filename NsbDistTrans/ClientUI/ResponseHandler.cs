using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Messages.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    class DataResponseMessageHandler : IHandleMessages<ResponseCommand>, IHandleMessages<ResponseEvent>, IHandleMessages<ResponseMessage>
    {
        static ILog log = LogManager.GetLogger<DataResponseMessageHandler>();
        

        public Task Handle(ResponseCommand message, IMessageHandlerContext context)
        {
            log.Info($"Response received with description: {message.DataId}");
            return Task.CompletedTask;
        }

        public Task Handle(ResponseEvent message, IMessageHandlerContext context)
        {
            log.Info($"Response received with description: {message.DataId}");
            return Task.CompletedTask;
        }

        public Task Handle(ResponseMessage message, IMessageHandlerContext context)
        {
            log.Info($"Response received with description: {message.Message}");
            return Task.CompletedTask;
        }
    }
}
