namespace FoodApp.Orders
{
    public interface IOrderEventsStore
    {
        Task<OrderEventMetadata> CreateOrderEventAsync(OrderEvent order);        
        Task CancelOrderAsync(Order Order);
    }
}