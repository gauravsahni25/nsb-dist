using Microsoft.EntityFrameworkCore;
using SqlAccess.Models;

namespace SqlAccess.DataAccess
{
    public class ToDoSqlContext : DbContext
    {
        public DbSet<ToDoModel> ToDos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
            optionsBuilder.UseSqlServer(connection);
        }
    }

    public class ToDoSqlContext2 : DbContext
    {
        public DbSet<ToDoModel> ToDos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo2;User Id=sa;pwd=Docker@123";
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
