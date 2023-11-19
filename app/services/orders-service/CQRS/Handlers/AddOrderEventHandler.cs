using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FoodApp
{
    public class AddOrderEventHandler : IRequestHandler<AddOrderEventCommand, OrderEventResponse>
    {
        private readonly IOrderEventsStore orderEventsStore;

        public AddOrderEventHandler(IOrderEventsStore eventStore)
        {
            orderEventsStore = eventStore;
        }

        public async Task<OrderEventResponse> Handle(AddOrderEventCommand request, CancellationToken cancellationToken)
        {                                    
            return await orderEventsStore.CreateOrderEventAsync(request.Event);
        }
    }
}