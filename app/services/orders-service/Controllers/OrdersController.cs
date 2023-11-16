using Microsoft.AspNetCore.Mvc;
using MediatR;
using Newtonsoft.Json;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender sender;
        private readonly IDaprEventBus eb;
        AILogger logger;

        public OrdersController(ISender sender,IDaprEventBus eventBus, AILogger aiLogger)
        {
            this.sender = sender;
            this.logger = aiLogger;
            this.eb = eventBus;
        }
        
        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<OrderEventResponse> CreateOrderEvent(Order order)
        {
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
            eb.Publish(new OrderEvent
            {
                OrderId = order.Id,
                CustomerId = order.Customer.Id,
                EventType = "payment-requested",
                Data = JsonConvert.SerializeObject(paymentRequest)
            });

            return resp;
        }

        // http://localhost:PORT/orders/events/add
        [HttpPost()]
        [Route("events/add")]
        public async Task<OrderEventResponse> AddOrderEvent(OrderEvent evt)
        {
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
