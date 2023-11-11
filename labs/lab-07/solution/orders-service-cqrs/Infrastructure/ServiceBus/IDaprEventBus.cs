namespace FoodApp
{
    public interface IDaprEventBus
    {
        void Publish(OrderEvent @event);
    }
}