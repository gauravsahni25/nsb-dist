using System;
using System.Threading.Tasks;
using System.Transactions;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using SqlAccess.DataAccess;
using SqlAccess.Models;

namespace MongoActor.Handlers
{
    public class DistributedTransactionHandler : IHandleMessages<StorageOps>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();

        public async Task Handle(StorageOps message, IMessageHandlerContext context)
        {
            var transactionStarted = new DistributedTransactionStarted()
            {
                OrderId = context.MessageId
            };
            await context.Publish(transactionStarted);
            log.Info($"DistributedTransaction, Id = {context.MessageId} - Started");
            
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                log.Info($"Received StorageOps");

                // Sql1
                using (var sqlContext = new ToDoSqlContext())
                {

                    ToDoModel todoModel = new ToDoModel()
                    {
                        Content = $"Mongo Actor Document with Id: {2}",
                        Title = $"MongoActor : {2}"
                    };

                    sqlContext.ToDos.Add(todoModel);
                    sqlContext.SaveChanges();
                }

                // Mongo
                var _todoMongoContext = new TodoMongoContext(new MongoDbConfig());
                var _todoMongoRepository = new TodoMongoRepository(_todoMongoContext);
                var id = await _todoMongoRepository.GetNextId(); // Bad Spot-1
                Todo todo = new Todo()
                {
                    Id = id,
                    Content = $"Mongo Actor Document with Id: {id}",
                    Title = $"MongoActor : {id}"
                };
                await _todoMongoRepository.Create(todo); // Bad Spot-2

                // sql2
                using (var sqlContext = new ToDoSqlContext2())
                {

                    ToDoModel todoModel = new ToDoModel()
                    {
                        Content = $"Mongo Actor Document with Id: {2}",
                        Title = $"MongoActor : {2}"
                    };

                    sqlContext.ToDos.Add(todoModel);
                    sqlContext.SaveChanges();
                }

                throw new Exception("Mongo Exception");

                var transactionEnded = new DistributedTransactionEnded()
                {
                    OrderId = context.MessageId,
                };
                await context.Publish(transactionEnded);
                log.Info($"DistributedTransaction, Id = {context.MessageId} - Ended"); 
                scope.Complete();
            }
        }
    }
}

//private static async Task RunLoop(IEndpointInstance endpointInstance)
//{
//    var todoContext = new TodoMongoContext(new MongoDbConfig());
//    var _repo = new TodoMongoRepository(todoContext);

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