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
}
