using MediatR;

namespace FoodApp.Orders
{
    public class GetOrdersByIdHandler : IRequestHandler<GetOrdersById, Order>
    {
        private readonly IOrderAggregates orderAggregates;

        public GetOrdersByIdHandler(IOrderAggregates aggregates)
        {
            orderAggregates = aggregates;
        }

        public async Task<Order> Handle(GetOrdersById request, CancellationToken cancellationToken)
        {
            return await orderAggregates.GetOrderAsync(request.orderId, request.CustomerId);
        }
    }
    
}