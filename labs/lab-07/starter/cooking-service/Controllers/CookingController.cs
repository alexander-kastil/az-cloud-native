using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class CookingController : ControllerBase
    {
        AILogger logger;
        ICookingRepository service;

        public CookingController(ICookingRepository cs, AILogger aILogger)
        {
            logger = aILogger;
            service = cs;
        }

        // http://localhost:PORT/payment/create
        [HttpPost()]
        [SwaggerOperation(Summary = "Create an order", Description = "Create an order")]
        [Route("create")]
        public async Task AddOrder(Order order)
        {
            await service.AddOrderAsync(order);
        }

        // http://localhost:5002/payment/getAll
        [HttpGet()]
        [SwaggerOperation(Summary = "Get all orders", Description = "Get all orders")]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await service.GetOrdersAsync();
        }

        // http://localhost:5002/payment/getById/{id}
        [HttpGet()]
        [SwaggerOperation(Summary = "Get and order by id", Description = "Get and order by id")]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            return await service.GetOrderAsync(id, customerId);
        }    
    }
}
