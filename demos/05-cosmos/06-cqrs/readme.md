# Understanding the CQRS Pattern

In this demo we will use `CQRS (Command Query Responsibility Segregation)` to separate the read and write models of our application. We will use the `MediatR` library to implement the CQRS pattern.

>Note: In order for this demo to work successfully, make sure that the [order-events-processor](../05-feed-event-sourcing/order-events-processor/) from the previous demo is running, otherwise the order will not be created. If you have the [Azure Function Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp) installed, you can run the function locally by running the following command from the `order-events-processor` directory:

```bash
func start
```

## Links & Resources

[MediatR](https://github.com/jbogard/MediatR)