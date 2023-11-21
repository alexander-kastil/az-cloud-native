using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Orders
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        AILogger logger;

        public OrdersController(AILogger ai)
        {
            logger = ai;            
        }

        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<OrderEventResponse> AddOrder(Order order)
        {
            logger.LogEvent("Order created", order.Id, true);

            // send a mock response
            return new OrderEventResponse
            {
                Id = Guid.NewGuid().ToString(),
                EventType = "OrderCreated",
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
        }

        // http://localhost:5002/orders/getAll
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            logger.LogEvent("Get all orders", "GetAllOrders", true);
            return null;
        }

        // http://localhost:5002/orders/getById/{id}/{customerId
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            logger.LogEvent("Get order by id", id, true);
            return null;
        }       
    }
}
