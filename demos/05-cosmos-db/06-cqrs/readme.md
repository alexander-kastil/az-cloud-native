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

```c#
public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
{
    private readonly IOrderAggregates orderAggregates;

    public GetOrdersHandler(IOrderAggregates aggregates)
    {
        orderAggregates = aggregates;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await orderAggregates.GetOrdersAsync();
    }
}
```
