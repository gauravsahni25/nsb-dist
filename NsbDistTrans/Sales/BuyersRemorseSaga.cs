using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales
{
    class BuyersRemorsePolicy : Saga<BuyersRemorsePolicy.BuyersRemorseState>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleTimeouts<BuyersRemorsePolicy.BuyersRemorseIsOver>,
        IHandleMessages<CancelOrder>
    {
        static ILog log = LogManager.GetLogger<BuyersRemorsePolicy>();


        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");
            Data.OrderId = message.OrderId;

            log.Info($"Starting cool down period for order #{Data.OrderId}.");
            await RequestTimeout(context, TimeSpan.FromSeconds(5), new BuyersRemorseIsOver()); // context, time to delay, actual message that will be sent whe the timeout is over
        }

        public async Task Timeout(BuyersRemorseIsOver state, IMessageHandlerContext context)
        {
            log.Info($"Cooling down period for order #{Data.OrderId} has elapsed.");

            var orderPlaced = new OrderPlaced
            {
                OrderId = Data.OrderId
            };

            await context.Publish(orderPlaced);

            MarkAsComplete();
        }
        public Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            log.Info($"Order #{message.OrderId} was cancelled.");

            //TODO: Possibly publish an OrderCancelled event?

            MarkAsComplete(); // Since the saga is complete now, all further messages will be ignored, thus nothing will happen once timeout is resolved.

            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BuyersRemorseState> mapper)
        {
            mapper
                .ConfigureMapping<PlaceOrder>(message => message.OrderId)
                .ToSaga(saga => saga.OrderId);
            mapper
                .ConfigureMapping<CancelOrder>(message => message.OrderId)
                .ToSaga(saga => saga.OrderId);
        }
        public class BuyersRemorseState : ContainSagaData
        {
            public string OrderId { get; set; }
        }
        public class BuyersRemorseIsOver
        {
        }
    }
}
