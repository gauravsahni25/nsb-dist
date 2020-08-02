using System.Collections.Generic;
using System.Threading.Tasks;
using Shipping.MongoStuffForBusiness.Models;

namespace Shipping.MongoStuffForBusiness
{
    public interface ITodoRepository
    {
        // api/[GET]
        Task<IEnumerable<Todo>> GetAllTodos();
        // api/1/[GET]
        Task<Todo> GetTodo(long id);
        // api/[POST]
        Task Create(Todo todo);
        // api/[PUT]
        Task<bool> Update(Todo todo);
        // api/1/[DELETE]
        Task<bool> Delete(long id);
        Task<long> GetNextId();
    }
}