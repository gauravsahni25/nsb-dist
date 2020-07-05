using NServiceBus;

namespace Messages.Commands
{
    public class PlaceOrder :
        ICommand
    {
        public string OrderId { get; set; }
    }
    public class ShipOrder : ICommand
    {
        public string OrderId { get; set; }
    }
    public class CancelOrder
        : ICommand
    {
        public string OrderId { get; set; }
    }
    public class HandlerLessCommand
        : ICommand
    {
        public string OrderId { get; set; }
    }

# warning Bad
    public class RequestCommand : ICommand
    {
        public int DataId { get; set; }
        public string Message { get; set; }
    }
#warning More Bad
    public class ResponseCommand : ICommand
    {
        public int DataId { get; set; }
        public string Message { get; set; }
    }
}
