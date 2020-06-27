using System;
using System.Threading.Tasks;
using Messages.Events;
using MongoActor.DataAccess;
using NServiceBus;
using NServiceBus.Logging;
using TodoApp.Models;

namespace MongoActor.Handlers
{
    public class DistributedTransactionHandler : IHandleMessages<OrderPlaced>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();

        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId} - Charging credit card...");
            
            var transactionStarted = new DistributedTransactionStarted()
            {
                OrderId = message.OrderId
            };
            await context.Publish(transactionStarted);
            log.Info($"DistributedTransaction, Id = {message.OrderId} - Started");

            // Mongo
            var todoContext = new TodoContext(new MongoDbConfig());
            var _repo = new TodoRepository(todoContext);
            var id = await _repo.GetNextId();
            Todo todo = new Todo()
            {
                Id = id,
                Content = $"Mongo Actor Document with Id: {id}",
                Title = $"MongoActor : {id}"
            };
            await _repo.Create(todo);
            // throw new Exception("Mongo Exception");


            var transactionEnded = new DistributedTransactionEnded()
            {
                OrderId = message.OrderId
            };
            await context.Publish(transactionEnded);
            log.Info($"DistributedTransaction, Id = {message.OrderId} - Ended");
        }
    }
}

//private static async Task RunLoop(IEndpointInstance endpointInstance)
//{
//    var todoContext = new TodoContext(new MongoDbConfig());
//    var _repo = new TodoRepository(todoContext);

//    while (true)
//    {
//        log.Info("Press 'G' to get all, 'A' to Add, or 'Q' to quit.");
//        var key = Console.ReadKey();
//        Console.WriteLine();
//        var id = await _repo.GetNextId();
//        switch (key.Key)
//        {
//            case ConsoleKey.G:
//                var results = await _repo.GetAllTodos();
//                break;

//            case ConsoleKey.A:
//                Todo todo = new Todo()
//                {
//                    Id = id,
//                    Content = $"Data and its id: {id}",
//                    Title = $"Title : {id}"
//                };
//                await _repo.Create(todo);
//                break;

//            case ConsoleKey.Q:
//                return;

//            default:
//                log.Info("Unknown input. Please try again.");
//                break;
//        }
//    }
//}