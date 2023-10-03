namespace FoodApp.Orders
{
    public interface IOrderAggregates
    {        
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderAsync(string id, string customerId);
        Task<IEnumerable<Order>> GetOrdersByQueryAsync(string query);
    }
}