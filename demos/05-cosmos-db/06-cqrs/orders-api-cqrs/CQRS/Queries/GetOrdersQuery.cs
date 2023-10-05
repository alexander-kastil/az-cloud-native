using MediatR;

namespace FoodApp.Orders
{
    public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
    
}