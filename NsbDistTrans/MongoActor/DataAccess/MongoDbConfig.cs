namespace MongoActor.DataAccess
{
    public class MongoDbConfig
    {
        public string Database { get; set; } = "TodoDB";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 27017;
        public string User { get; set; } = "root";
        public string Password { get; set; } = "Docker123";
        public string ConnectionString
        {
            get
            {
                //if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
                //    return $@"mongodb://{Host}:{Port}";
                return $@"mongodb://{User}:{Password}@{Host}:{Port}";
            }
        }
    }
}