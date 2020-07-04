using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SqlAccess.Models
{
    public class ToDoModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
    public class Todo
    {
        [BsonId]
        public ObjectId InternalId { get; protected set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}