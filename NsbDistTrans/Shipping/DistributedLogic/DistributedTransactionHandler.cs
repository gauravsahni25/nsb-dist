using System;
using System.Threading.Tasks;
using Messages.Events;
using MongoDB.Bson;
using MongoDB.Driver;
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
            var session = context.SynchronizedStorageSession.GetClientSession();
            
            var collection = session
                .Client
                .GetDatabase("Shipping")
                .GetCollection<Todo>("Todos");

            var id = new Guid();

            var businessObject = new Todo()
            {
                Id = id,
                Content = $"Mongo Actor Document with Id: {id}",
                Title = $"MongoActor : {id}"
            };
            await collection.InsertOneAsync(session, businessObject);
            
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