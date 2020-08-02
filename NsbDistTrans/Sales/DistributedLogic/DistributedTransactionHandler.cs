using System;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using Sales.SqlStuffForBusiness;
using Sales.SqlStuffForBusiness.Models;

namespace Sales.DistributedLogic
{
    public class DistributedTransactionHandler : IHandleMessages<BusinessEvent>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();

        public async Task Handle(BusinessEvent message, IMessageHandlerContext context)
        {
            ToDoSqlContext _sqlContext = context.DataContext();
            log.Info($"Received OrderPlaced, OrderId = {message.EventId} - Charging credit card...");
            
            var transactionStarted = new DistributedTransactionStarted()
            {
                OrderId = message.EventId
            };
            await context.Publish(transactionStarted);
            log.Info($"DistributedTransaction, Id = {message.EventId} - Started");

            // Sql
            ToDoModel todoModel = new ToDoModel()
            {
                Content = $"Mongo Actor Document with Id: {message.EventId}",
                Title = $"MongoActor : {message.EventId}"
            };

            _sqlContext.ToDos.Add(todoModel);
            await _sqlContext.SaveChangesAsync();

            //throw new Exception("Mongo Exception");


            var transactionEnded = new DistributedTransactionEnded()
            {
                OrderId = message.EventId
            };
            await context.Publish(transactionEnded);
            log.Info($"DistributedTransaction, Id = {message.EventId} - Ended");
        }
    }
}
