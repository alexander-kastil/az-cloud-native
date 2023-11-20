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
        IOrdersRepository orders;

        public OrdersController(AILogger ai)
        {
            logger = ai;            
        }

        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task AddOrder(Order order)
        {
            logger.LogEvent("Order created", order.Id,true);
            // await orders.AddOrderAsync(order);
        }

        // http://localhost:5002/orders/getAll
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await orders.GetOrdersAsync();
        }

        // http://localhost:5002/orders/getById/{id}/{customerId
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            return await orders.GetOrderAsync(id, customerId);
        }

        // http://localhost:5002/orders/delete/{id}/{customerId
        [HttpDelete()]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrder(Order order)
        {
            await orders.DeleteOrderAsync(order);
            return Ok();
        }
    }
}
