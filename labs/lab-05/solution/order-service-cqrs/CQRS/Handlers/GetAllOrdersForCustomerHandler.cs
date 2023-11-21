using MediatR;

namespace FoodApp
{
    public class GetAllOrdersForCustomerHandler : IRequestHandler<GetAllOrdersForCustomer, IEnumerable<Order>>
    {
        private readonly IOrderAggregates orderAggregates;

        public GetAllOrdersForCustomerHandler(IOrderAggregates aggregates)
        {
            orderAggregates = aggregates;
        }

        public async Task<IEnumerable<Order>> Handle(GetAllOrdersForCustomer request, CancellationToken cancellationToken)
        {
            return await orderAggregates.GetAllOrdersForCustomer(request.CustomerId);
        }
    }    
}