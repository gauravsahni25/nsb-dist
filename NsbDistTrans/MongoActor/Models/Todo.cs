namespace TodoApp.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Todo
    {
        [BsonId]
        public ObjectId InternalId { get; protected set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}