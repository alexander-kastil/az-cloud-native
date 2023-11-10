using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace FoodApp.Orders
{
    public class OrdersRepository : IOrdersRepository
    {
        public OrdersRepository(){}
        
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {          
            return await Task.FromResult(new List<Order>());
        }

        public async Task<Order> GetOrderAsync(string id, string customerId)
        {
            return await Task.FromResult(new Order());
        }

        public async Task<IActionResult> AddOrderAsync(Order item)
        {
            return await Task.FromResult(new OkResult());
        }

        public async Task<IActionResult> DeleteOrderAsync(Order item)
        {
            return await Task.FromResult(new OkResult());
        }
    }
}