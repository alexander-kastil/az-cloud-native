using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender sender;

        public OrdersController(ISender sender)
        {
            this.sender = sender;
        }
        
        // http://localhost:PORT/orders/create
        [HttpPost()]
        [Route("create")]
        public async Task<OrderEventResponse> AddOrder(Order order)
        {
            return await sender.Send(new AddOrderCommand(order));
        }

        // http://localhost:5002/orders/getOrders
        [HttpGet()]
        [Route("getOrders")]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await sender.Send(new GetOrdersQuery());
        }

        // http://localhost:5002/orders/getById/{id}/{customerId}
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<Order> GetOrderById(string orderId, string customerId)
        {
            return await sender.Send(new GetOrdersById(orderId, customerId));
        }
    }
}
