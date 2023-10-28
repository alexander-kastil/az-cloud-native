using MediatR;

namespace FoodApp
{
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
}