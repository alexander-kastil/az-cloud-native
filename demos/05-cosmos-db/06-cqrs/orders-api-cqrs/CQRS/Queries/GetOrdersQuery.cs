using MediatR;

namespace FoodApp.Orders.Queries
{
    public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
    public record GetOrdersById(string orderId, string CustomerId) : IRequest<Order>;
    
}