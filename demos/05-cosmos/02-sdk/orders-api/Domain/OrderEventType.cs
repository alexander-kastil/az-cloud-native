namespace FoodApp
{
    public enum OrderEventType
    {
        OrderCreated,
        PaymentRequested,
        PaymentSuccess,
        PaymentFailed,
        ProductionRequested,
        ProductionStarted,
        ProductionCompleted,
        ProductionNotCompleted,
        DeliveryStarted,
        DeliveryCompleted,
        OrderCompleted,
        OrderCanceled
    }
}