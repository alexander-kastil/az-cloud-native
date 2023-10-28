# Cosmos DB Change Feed and Event Sourcing

In this demo we will use a Cosmos DB Container `order-events` as event store for an Event Sourcing pattern. In order to allow fast inserts we will use a partition key of `/id`. We will then use the Cosmos DB Change Feed to project the events into a second container `orders` which will be used as the read model for the application and create an order aggregate that contains the current state of the order as all the events that have been applied to it.

This demo consists of two projects:

- `order-event-store` - A .NET Api that will be used to insert events into the `order-events` container.
- `order-event-processor` - An Azure Function that will process the change feed and project the events into the `orders` container.

>Note: Event Sourcing is also available for Azure SQL Databases but is not covered in this class.

## Links & Resources

[Event Sourcing pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
