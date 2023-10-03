namespace FoodApp.Orders
{
    public interface IEvent
    {
        DateTime Timestamp { get; }
    }
}