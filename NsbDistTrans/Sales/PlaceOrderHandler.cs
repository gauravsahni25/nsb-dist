//using System;
//using System.Threading.Tasks;
//using Messages.Commands;
//using Messages.Events;
//using NServiceBus;
//using NServiceBus.Logging;

//namespace Sales
//{
//    public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
//    {
//        static Random random = new Random();
//        static ILog log = LogManager.GetLogger<PlaceOrderHandler>();

//        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
//        {
//            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");

//            // To see Poison message demo
//            // throw new Exception("BOOM");
            
//            // To see transient error demo
//            //if (random.Next(0, 5) == 0)
//            //{
//            //    throw new Exception("Oops");
//            //}

//            var orderPlaced = new OrderPlaced
//            {
//                OrderId = message.OrderId
//            };
//            return context.Publish(orderPlaced);
//        }
//    }
//}
