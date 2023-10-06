using MediatR;

namespace FoodApp.Orders
{
    public record GetOrdersById(string orderId, string CustomerId) : IRequest<Order>;
    
}