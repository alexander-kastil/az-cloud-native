using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodApp
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
        [SwaggerOperation(Summary = "Create an order", Description = "Create an order")]
        [Route("create")]
        public async Task AddOrder(Order order)
        {
            await service.AddOrderAsync(order);
        }

        // http://localhost:5002/orders/getAll
        [HttpGet()]
        [SwaggerOperation(Summary = "Get all orders", Description = "Get all orders")]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await service.GetOrdersAsync();
        }

        // http://localhost:5002/orders/getById/{id}/{customerId
        [HttpGet()]
        [SwaggerOperation(Summary = "Get and order by id", Description = "Get and order by id")]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            return await service.GetOrderAsync(id, customerId);
        }

        // http://localhost:5002/orders/update
        [HttpPut()]
        [SwaggerOperation(Summary = "Update an order", Description = "Update an order")]
        [Route("update")]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            await service.UpdateOrderAsync(order.Id, order);
            return Ok();
        }

        // http://localhost:5002/orders/delete/{id}/{customerId
        [HttpDelete()]
        [SwaggerOperation(Summary = "Delete an order", Description = "Delete an order")]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrder(Order order)
        {
            await service.DeleteOrderAsync(order);
            return Ok();
        }
    }
}
