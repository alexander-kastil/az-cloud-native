using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Orders
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        AILogger logger;
        IOrdersRepository service;

        public OrdersController(IOrdersRepository cs, AILogger aILogger)
        {
            logger = aILogger;
            service = cs;
        }

        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task AddOrder(Order order)
        {
            await service.AddOrderAsync(order);
        }

        // http://localhost:5002/orders/getAll
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await service.GetOrdersAsync();
        }

        // http://localhost:5002/orders/getById/{id}/{customerId
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            return await service.GetOrderAsync(id, customerId);
        }

        // http://localhost:5002/orders/delete/{id}/{customerId
        [HttpDelete()]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrder(Order order)
        {
            await service.DeleteOrderAsync(order);
            return Ok();
        }
    }
}
