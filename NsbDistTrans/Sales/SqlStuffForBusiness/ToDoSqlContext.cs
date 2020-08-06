using Microsoft.EntityFrameworkCore;
using Sales.SqlStuffForBusiness.Models;

namespace Sales.SqlStuffForBusiness
{
    public class ToDoSqlContext : DbContext
    {
        public ToDoSqlContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ToDoModel> ToDos { get; set; }
    }

    public class ToDoSqlContextCreation : DbContext
    {
        public DbSet<ToDoModel> ToDos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
