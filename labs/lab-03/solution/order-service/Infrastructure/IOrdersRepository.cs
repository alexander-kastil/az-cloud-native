using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Orders
{
    public interface IOrdersRepository
    {        
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderAsync(string id, string customerId);
        Task<IActionResult> AddOrderAsync(Order Order);
        Task<IActionResult> DeleteOrderAsync(Order Order);
    }
}