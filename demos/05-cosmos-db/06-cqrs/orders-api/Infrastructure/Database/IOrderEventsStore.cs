namespace FoodApp.Orders
{
    public interface IOrderEventsStore
    {
        Task<string> CreateOrderEventAsync(OrderEvent order);        
        Task CancelOrderAsync(Order Order);
    }
}