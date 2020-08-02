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
}