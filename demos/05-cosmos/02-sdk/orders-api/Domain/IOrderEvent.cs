using System;

namespace FoodApp
{
    public interface IOrderEvent
    {
        string Id { get; }
        Customer Customer { get; }
    }
}