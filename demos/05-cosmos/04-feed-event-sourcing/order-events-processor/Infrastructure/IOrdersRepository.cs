using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp
{
    public interface IOrdersRepository
    {        
        Task<Order> GetOrderAsync(string id, string customerId);
        Task CreateOrderAsync(Order Order);
        Task UpdateOrderAsync(string id, Order Order);
    }
}