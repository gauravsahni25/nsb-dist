using MongoDB.Driver;
using TodoApp.Models;

namespace MongoActor.DataAccess
{
    public interface ITodoContext
    {
        IMongoCollection<Todo> Todos { get; }
    }
    public class TodoContext : ITodoContext
    {
        private readonly IMongoDatabase _db;
        public TodoContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
        public IMongoCollection<Todo> Todos => _db.GetCollection<Todo>("Todos");
    }
}