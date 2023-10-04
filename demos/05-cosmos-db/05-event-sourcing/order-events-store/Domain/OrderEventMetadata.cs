namespace FoodApp.Orders
{
    public class OrderEventMetadata{
        public string Id { get; set; }
        public string EventType { get; set; }
        public string OrderId { get; set; }
        public string CustomerId { get; set; }     
        public DateTime Timestamp { get; set; }   
    }
}