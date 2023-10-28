using MediatR;

namespace FoodApp
{
    public record GetOrdersQuery : IRequest<IEnumerable<Order>>;    
}