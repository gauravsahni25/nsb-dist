using MongoDB.Driver;
using SqlAccess.Models;

namespace SqlAccess.DataAccess
{
    public interface ITodoContext
    {
        IMongoCollection<Todo> Todos { get; }
    }
    public class TodoMongoContext : ITodoContext
    {
        private readonly IMongoDatabase _db;
        public TodoMongoContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
        public IMongoCollection<Todo> Todos => _db.GetCollection<Todo>("Todos");
    }
}