using MediatR;

namespace FoodApp
{
    public record CreateEventCommand(OrderEvent Event) : IRequest<string>;
}