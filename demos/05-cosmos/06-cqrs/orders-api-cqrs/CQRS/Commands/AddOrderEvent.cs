using MediatR;

namespace FoodApp.Orders
{
    public record AddOrderCommand(Order order) : IRequest<OrderEventMetadata>;
}