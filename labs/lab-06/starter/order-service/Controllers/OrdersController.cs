using Microsoft.AspNetCore.Mvc;
using MediatR;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender sender;
        private readonly IDaprEventBus eb;
        AILogger logger;

        public OrdersController(ISender sender,IDaprEventBus bus, AILogger ai)
        {
            this.sender = sender;
            this.logger = ai;
            this.eb = bus;
        }
        
        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<OrderEventResponse> CreateOrderEvent(Order order)
        {
            logger.LogEvent("CreateOrderEvent", order);
            var resp = await sender.Send(new CreateOrderEventCommand(order));

            // Created the Payment Request
            // This could also be done in the CreateOrderEventHandler
            // We do it here to make it comparable with the previous lab
            var paymentRequest = new PaymentRequest
            {
                OrderId = order.Id,
                Amount = order.Total,
                PaymentInfo = order.Payment
            };

             // Wrap it into our Integration Event
            logger.LogEvent("PaymentRequestEvent", paymentRequest);
            return resp;
        }

        // http://localhost:PORT/orders/events/add
        [HttpPost()]
        [Route("events/add")]
        public async Task<OrderEventResponse> AddOrderEvent(OrderEvent evt)
        {
            logger.LogEvent("AddOrderEvent", evt);
            return await sender.Send(new AddOrderEventCommand(evt));
        }

        // http://localhost:PORT/orders/getById/{orderId}/{customerId}
        [HttpGet()]
        [Route("getById/{orderId}/{customerId}")]
        public async Task<Order> GetOrderById(string orderId, string customerId)
        {
            return await sender.Send(new GetOrdersById(orderId, customerId));
        }

        // http://localhost:PORT/orders/getForCustomer/{customerId}
        [HttpGet()]
        [Route("getAllOrdersForCustomer/{customerId}")]
        public async Task<IEnumerable<Order>> GetAllOrdersForCustomer(string orderId, string customerId)
        {
            return await sender.Send(new GetAllOrdersForCustomer(customerId));
        }
    }
}
