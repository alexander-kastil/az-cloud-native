namespace FoodApp.Orders
{
    public interface IOrderEventsStore
    {
        Task<IEnumerable<Order>> GetOrdersAsync(string query);
        Task<Order> GetOrderAsync(string id, string customerId);
        Task<OrderEventMetadata> CreateOrderEventAsync(OrderEvent order);        
    }
}