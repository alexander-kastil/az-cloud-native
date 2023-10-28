using MediatR;

namespace FoodApp
{
    public record AddOrderCommand(Order order) : IRequest<OrderEventMetadata>;
}