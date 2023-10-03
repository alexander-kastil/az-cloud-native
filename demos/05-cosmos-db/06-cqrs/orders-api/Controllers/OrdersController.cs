using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace FoodApp.Orders
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IWebHostEnvironment env;
        IOrderAggregates service;

        public OrdersController(IWebHostEnvironment environment, IOrderAggregates cs, AILogger aILogger)
        {
            env = environment;
            service = cs;
        }

        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task AddOrder(Order order)
        {
            // await service.AddOrderAsync(order);
        }

        // http://localhost:5002/orders/getOrders
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await service.GetOrdersAsync();
        }

        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string id, string customerId)
        {
            return await service.GetOrderAsync(id, customerId);
        }


        [HttpPut()]
        [Route("update")]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            // await service.UpdateOrderAsync(order.Id, order);
            return Ok();
        }
    }
}
