using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    public class ShippingPolicy : Saga<ShippingPolicy.ShippingPolicyData>,
        IAmStartedByMessages<OrderPlaced>, // I can start the saga!
        IAmStartedByMessages<OrderBilled>  // I can start the saga too!
    {
        static ILog log = LogManager.GetLogger<ShippingPolicy>();

        public class ShippingPolicyData : ContainSagaData
        {
            public bool IsOrderPlaced { get; set; }
            public bool IsOrderBilled { get; set; }
            public string OrderId { get; set; } // To find saga from both messages
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
        {
            mapper
                .ConfigureMapping<OrderPlaced>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
            mapper
                .ConfigureMapping<OrderBilled>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
        }

        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"OrderPlaced message received.");
            Data.IsOrderPlaced = true;
            return ProcessOrder(context);
        }
        public Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            log.Info($"OrderBilled message received.");
            Data.IsOrderBilled = true;
            return ProcessOrder(context);
        }
        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderPlaced && Data.IsOrderBilled)
            {
                await context.SendLocal(new ShipOrder() { OrderId = Data.OrderId });
                MarkAsComplete();
            }
        }
    }
}