using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodApp
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
        public async Task<OrderEventResponse> CreateOrderEvent(Order order)
        {
            var evt = new OrderEvent
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = order.Customer.Id,
                EventType = "Created",
                Data = order
            };
            order.Id = evt.OrderId;
            return await store.AddEventAsync(evt);
        }

        // http://localhost:PORT/orderevent/addEvent
        [HttpPost()]
        [SwaggerOperation(Summary = "Add an order event", Description = "Adds an order event to the event store")]
        [Route("add")]
        public async Task<OrderEventResponse> AddOrderEvent(OrderEvent evt)
        {   
            // fix deserialization otherwise Data will contain "ValueKind": 1     
            object objData = JsonConvert.DeserializeObject<dynamic>(evt.Data.ToString());
            evt.Data = objData;
            // add to event store
            return await store.AddEventAsync(evt);
        }

        // http://localhost:5002/orders/getForOrder/{OrderId}
        [HttpGet()]
        [SwaggerOperation(Summary = "Gets all order events for a given order", Description = "Gets all order events for a given order")]
        [Route("getForOrder/{OrderId}")]
        public Order[] GetForOrder(int OrderId)
        {
            var orders = store.GetAllEventsForOrder($"SELECT * FROM  order-events eo where eo.orderId={OrderId}").Result;
            return orders.ToArray();
        }       
    }
}
