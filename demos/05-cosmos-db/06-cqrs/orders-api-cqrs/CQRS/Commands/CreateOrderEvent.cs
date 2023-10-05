using MediatR;

namespace FoodApp.Orders
{
    public record CreateEventCommand(OrderEvent Event) : IRequest<string>;
}