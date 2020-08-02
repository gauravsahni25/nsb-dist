using MongoDB.Driver;
using Shipping.MongoStuffForBusiness.Models;

namespace Shipping.MongoStuffForBusiness
{
    public interface ITodoContext
    {
        IMongoCollection<Todo> Todos { get; }
    }
    public class TodoMongoContext : ITodoContext
    {
        private readonly IMongoDatabase _db;
        public TodoMongoContext(MongoDbConfig config, IMongoClient mongoClient)
        {
            _db = mongoClient.GetDatabase(config.Database);
        }
        public IMongoCollection<Todo> Todos => _db.GetCollection<Todo>("Todos");
    }
}