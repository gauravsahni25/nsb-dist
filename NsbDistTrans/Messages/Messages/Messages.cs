using NServiceBus;

namespace Messages.Messages
{
#warning Kind of Okay! But really???
    public class ResponseMessage : IMessage
    {
        public int DataId { get; set; }
        public string Message { get; set; }
    }
    public class RequestMessage : IMessage
    {
        public int DataId { get; set; }
        public string Message { get; set; }
    }
}
