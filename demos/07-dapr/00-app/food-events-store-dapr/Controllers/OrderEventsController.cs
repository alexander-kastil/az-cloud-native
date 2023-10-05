using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodApp.Orders
{
    [Route("[controller]")]
    [ApiController]
    public class OrderEventController : ControllerBase
    {
        AILogger logger;
        IOrderEventsStore store;

        public OrderEventController(IOrderEventsStore cs,  AILogger aILogger)
        {
            logger = aILogger;
            store = cs;
        }

        // Takes an order and wraps it in an initial OrderEvent
        // http://localhost:PORT/orderevent/create
        [HttpPost()]
        [SwaggerOperation(Summary = "Create an order event", Description = "Takes an order and wraps it in an initial OrderEvent")]
        [Route("create")]
        public async Task<OrderEventMetadata> CreateOrderEvent(Order order)
        {
            var @event = new OrderEvent(order.Id, order.CustomerId, OrderEventType.OrderCreated.ToString(), order);
            return await store.CreateOrderEventAsync(@event);
        }

        // http://localhost:PORT/orderevent/addEvent
        [HttpPost()]
        [SwaggerOperation(Summary = "Add an order event", Description = "Adds an order event to the event store")]
        [Route("add")]
        public async Task<OrderEventMetadata> AddOrderEvent(OrderEvent @event)
        {
            return await store.CreateOrderEventAsync(@event);
        }

        // http://localhost:5002/orders/getForOrder/{OrderId}
        [HttpGet()]
        [SwaggerOperation(Summary = "Gets all order events for a given order", Description = "Gets all order events for a given order")]
        [Route("getForOrder/{OrderId}")]
        public Order[] GetForOrder(int OrderId)
        {
            var orders = store.GetOrdersAsync($"SELECT * FROM  order-events eo where eo.orderId={OrderId}").Result;
            return orders.ToArray();
        }
    }
}