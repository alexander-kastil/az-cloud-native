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
        IOrderEventsStore service;

        public OrdersController(IConfiguration config, IWebHostEnvironment environment, CosmosClient cosmosClient, IOrderEventsStore cs,  AILogger aILogger)
        {
            cfg = config.Get<AppConfig>(); ;
            env = environment;
            client = cosmosClient;
            logger = aILogger;
            service = cs;
        }

        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<string> CreateOrderEvent(Order order)
        {
            var @event = new OrderEvent(order.Id, OrderEventType.OrderCreated.ToString(), order);
            string id = await service.CreateOrderEventAsync(@event);
            return id;
        }

        [HttpPost()]
        [Route("add")]
        public async Task<string> AddOrderEvent(OrderEvent @event)
        {
            string id = await service.CreateOrderEventAsync(@event);
            return id;
        }

        [HttpGet()]
        [Route("cancel/{id}")]
        public async Task<string> CancelOrder(string id)
        {
            var order = await service.GetOrderAsync(id, id);
            if (order == null)
            {
                return "Order not found";
            }
            await service.CancelOrderAsync(order);
            return $"Order with id {id} cancelled";
        }

        // use cosmos client
        // http://localhost:5002/orders/getOrders
        [HttpGet()]
        [Route("getOrders")]
        public Order[] GetAllEventsForOrders(int OrderId)
        {
            var orders = service.GetOrdersAsync($"SELECT * FROM  order-events eo where eo.orderId={OrderId}").Result;
            return orders.ToArray();
        }
    }
}