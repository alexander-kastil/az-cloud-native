# Understanding the CQRS Pattern

## Links & Resources

[MediatR](https://github.com/jbogard/MediatR)

## CQRS with MediatR



```c#
namespace FoodApp.Orders.Queries
{
    public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
    public record GetOrdersById(string orderId, string CustomerId) : IRequest<Order>;
    
}
```