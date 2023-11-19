using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp
{
    public interface ICookingRepository
    {        
        Task AddOrderAsync(Order Order);
        Task UpdateOrderStatusAsync(string id, Order Order);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderAsync(string id, string customerId);
    }
}