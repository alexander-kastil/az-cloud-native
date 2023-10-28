using MediatR;

namespace FoodApp
{
    public class AddOrderEventHandler : IRequestHandler<AddOrderCommand, OrderEventMetadata>
    {
        private readonly IOrderEventsStore orderEventsStore;

        public AddOrderEventHandler(IOrderEventsStore eventStore)
        {
            orderEventsStore = eventStore;
        }

        public async Task<OrderEventResponse> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {            
            var evt = new OrderEvent(request.order.Id, request.order.CustomerId, OrderEventType.OrderCreated.ToString(), request.order);
            return await orderEventsStore.CreateOrderEventAsync(evt);
        }        
    }
}