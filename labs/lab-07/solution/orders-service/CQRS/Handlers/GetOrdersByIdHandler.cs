using MediatR;

namespace FoodApp
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
            return await orderAggregates.GetOrderByIdAsync(request.orderId, request.CustomerId);
        }
    }    
}