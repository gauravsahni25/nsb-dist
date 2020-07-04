using NServiceBus;

namespace Messages.Events
{
    public class OrderPlaced : IEvent
    {
        public string OrderId { get; set; }
    }

    public class OrderBilled : IEvent
    {
        public string OrderId { get; set; }
    }

    public class DistributedTransactionStarted : IEvent
    {
        public string OrderId { get; set; }
    }
    public class DistributedTransactionEnded : IEvent
    {
        public string OrderId { get; set; }
    }

    #warning Badly Named Event.  
    public class DuplicatesToPrevent : IEvent
    {
    }

    public class StorageOps : IEvent
    { }
}
