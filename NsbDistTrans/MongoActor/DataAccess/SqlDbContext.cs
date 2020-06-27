using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.Models;

namespace MongoActor.DataAccess
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
}
