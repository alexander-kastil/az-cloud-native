using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender sender;
        AILogger logger;

        public OrdersController(ISender sender,  AILogger aiLogger)
        {
            this.sender = sender;
            this.logger = aiLogger;
        }
        
        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<OrderEventResponse> CreateOrderEvent(Order order)
        {
            return await sender.Send(new CreateOrderEventCommand(order));
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
