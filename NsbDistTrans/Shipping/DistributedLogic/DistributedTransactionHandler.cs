using System;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using Shipping.MongoStuffForBusiness;
using Shipping.MongoStuffForBusiness.Models;

namespace Shipping.DistributedLogic
{
    public class DistributedTransactionHandler : IHandleMessages<BusinessEvent>
    {
        static ILog log = LogManager.GetLogger<DistributedTransactionHandler>();

        public async Task Handle(BusinessEvent message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.EventId} - Charging credit card...");
            
            var transactionStarted = new DistributedTransactionStarted()
            {
                OrderId = message.EventId
            };
            await context.Publish(transactionStarted);
            log.Info($"DistributedTransaction, Id = {message.EventId} - Started");

            // Mongo
            var _todoMongoContext = new TodoMongoContext(new MongoDbConfig());
            var _todoMongoRepository = new TodoMongoRepository(_todoMongoContext);
            var id = await _todoMongoRepository.GetNextId();
            Todo todo = new Todo()
            {
                Id = id,
                Content = $"Mongo Actor Document with Id: {id}",
                Title = $"MongoActor : {id}"
            };
            await _todoMongoRepository.Create(todo);
           
            throw new Exception("Mongo Exception");


            var transactionEnded = new DistributedTransactionEnded()
            {
                OrderId = message.EventId
            };
            await context.Publish(transactionEnded);
            log.Info($"DistributedTransaction, Id = {message.EventId} - Ended");
        }
    }
}