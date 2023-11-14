using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp
{
    public interface IOrderEventsStore
    {
        Task<IEnumerable<Order>> GetAllEventsForOrder(string query);
        Task<Order> GetOrderAsync(string id, string customerId);
        // Task<OrderEventResponse> CreateEventFromOrderAsync(OrderEvent order);        
        Task<OrderEventResponse> AddEventAsync(OrderEvent evt);
    }
}