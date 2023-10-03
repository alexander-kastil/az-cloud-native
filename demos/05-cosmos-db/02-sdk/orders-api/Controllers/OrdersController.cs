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
        AppConfig cfg;
        IWebHostEnvironment env;
        CosmosClient client;
        AILogger logger;
        IOrderRepository service;

        public OrdersController(IConfiguration config, IWebHostEnvironment environment, CosmosClient cosmosClient, IOrderRepository cs,  AILogger aILogger)
        {
            cfg = config.Get<AppConfig>(); ;
            env = environment;
            client = cosmosClient;
            logger = aILogger;
            service = cs;
        }

        // http://localhost:PORT/orders/add
        [HttpPost()]
        [Route("add")]
        public async Task AddOrder(Order order)
        {
            // using a repository pattern
            await service.AddOrderAsync(order);
        }

        // http://localhost:5002/orders/getOrders
        [HttpGet()]
        [Route("getOrders")]
        public  async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await service.GetOrdersAsync();
        }
    }
}