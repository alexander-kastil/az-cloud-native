# Dapr Service Invocation, Pub / Sub & Bindings

## Dapr Service Invocation

- [food-api-dapr](../00-app/food-api-dapr) is a REST API that exposes a set of endpoints to manage food items.

    ```c#
    [HttpGet()]
    public IEnumerable<FoodItem> GetFood()
    {
        return ctx.Food.ToArray();
    }
    ```

- This method is consumed by [food-mvc-dapr](../00-app/food-mvc-dapr/) Examine its [Program.cs](../00-app/food-invoices-dapr/Program.cs) and notice the following code:

    ```c#
    builder.Services.AddDapr();
    ...
    app.UseCloudEvents();
    ...
    app.MapSubscribeHandler();    
    ```

- `AddDapr()` registers the necessary services to integrate Dapr into the MVC pipeline. It also registers a `DaprClient` instance into the dependency injection container. 
- `UseCloudEvents()` adds CloudEvents middleware into the ASP.NET Core middleware pipeline. This middleware will unwrap requests that use the CloudEvents structured format, so the receiving method can read the event payload directly.
- `MapSubscribeHandler()` registers a route handler for the `dapr/subscribe` endpoint. This endpoint is used by Dapr to register the subscriber with the pub/sub component. The route handler will read the topic name from the request and register the subscriber with the pub/sub component.    

- Examine the current state of [HomeController.cs](../00-app/food-mvc-dapr/Controllers/HomeController.cs) and notice that it is using direct service invocation to get the food items:
    
    ```c#
    public async Task<IActionResult> Index()
    {
        HttpClient client = new HttpClient();
        var daprResponse = await client.GetAsync($"http://localhost:{BACKEND_PORT}/v1.0/invoke/{BACKEND_NAME}/method/food");
        var jsonFood = await daprResponse.Content.ReadAsStringAsync();
        ViewBag.Food =  JsonSerializer.Deserialize<List<FoodItem>>(jsonFood);;
        return View();
    }
    ```

    >Note: The reason why we are not consuming the REST-API directly is because we want to use the benefits of Dapr service invocation which are:
    - Service discovery
    - Standardizing API calls between services.
    - Secure inter-service communication.
    - Mitigating request timeouts or failures and automatic handling of retries and transient errors
    - Implementing observability and tracing using OpenTelemetry

        ![dapr-service-invocation](_images/dapr-service-invocation.png)

- Run the UI and test the implementation:

    ```bash
    cd food-dapr-fronted
    dapr run --app-id food-fronted --app-port 5002 --dapr-http-port 5011 dotnet run
    ```

- It should return the following result:

    ![food-app](_images/food-app.png)

## Dapr Pub/Sub

Dapr pub/sub building block provides a platform-agnostic API framework to send and receive messages. The publisher services publish messages to a named topic. Your consumer services subscribe to a topic to consume messages:


- `pubsub.yaml`:

    ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
    name: food-pubsub
    spec:
    type: pubsub.redis
    version: v1
    metadata:
        - name: redisHost
        value: localhost:6379
        - name: redisPassword
        value: ""
    ```
    ![pub-sub](_images/dapr-pub-sub.png)

### Publisher    

- Examine [FoodController.cs](../00-app/food-api-dapr/Controllers/FoodController.cs) 

    ```c#
    [HttpPost("add")]
    public async Task<FoodItem> AddFood([FromBody] FoodItem food)
    {
        logger.LogInformation("Started processing message with food name '{0}'", food.Name);
        var existing = ctx.Food.FirstOrDefault(f => f.ID == food.ID);
        if (existing != null)
        {
            ctx.Attach(food); 
            ctx.Entry(food).State = EntityState.Modified;
        }
        else
        {
            ctx.Food.Add(food);
            logger.LogInformation("Food with ID '{0}' does not exist. Adding it", food.ID);
        }
        await ctx.SaveChangesAsync();
        await PublishFoodAdded(food);
        return food;
    }
    ```

- Examine `PublishFoodAdded()`. It is responsible for publishing the food item to the Dapr Pub/Sub component:

    ```c#
    private async Task PublishFoodAdded(FoodItem food)
    {
        var pubsubName = cfg.GetValue<string>("PUBSUB_NAME");
        var topicName = cfg.GetValue<string>("PUBSUB_TOPIC");            
        await client.PublishEventAsync(pubsubName, topicName, food);
    }
    ```

    >Note: The `PublishEventAsync` method is used to publish the food item to the pub/sub component. `food-pubsub` is the name of the pub/sub component and `food-items` is the topic name.

- Run the api with Dapr and add the pub/sub component from the components folder:

    ```bash
    dapr run --app-id food-fronted --app-port 5002 --dapr-http-port 5011 --resources-path './components' dotnet watch run       
    ```

    >Note: The `--resources-path` parameter is used to specify the location of the components folder. It adds all the components of the folder to the app. Previously we used `--components-path` to add components. In `tasks.json` this is still the case.

### Subscriber

- [food-invoices-dapr](../00-app/food-invoices-dapr/) subscribes to the topic. Examine its [Program.cs](../00-app/food-invoices-dapr/Program.cs) and notice the following code:

    ```c#
    builder.Services.AddDapr();
    ...
    app.UseCloudEvents();
    ...
    app.MapSubscribeHandler();    
    ```

- `AddDapr()` registers the necessary services to integrate Dapr into the MVC pipeline. It also registers a `DaprClient` instance into the dependency injection container. 
- `UseCloudEvents()` adds CloudEvents middleware into the ASP.NET Core middleware pipeline. This middleware will unwrap requests that use the CloudEvents structured format, so the receiving method can read the event payload directly.
- `MapSubscribeHandler()` registers a route handler for the `dapr/subscribe` endpoint. This endpoint is used by Dapr to register the subscriber with the pub/sub component. The route handler will read the topic name from the request and register the subscriber with the pub/sub component.  

- [InvoicesController](../00-app/food-invoices-dapr/Controllers/InvoiceController.cs) is responsible for receiving the food items and creating invoices:

    ```c#
    [HttpPost]
        [Dapr.Topic("food-pubsub", "food-items")]
        public ActionResult CreateInvoice([FromBody] FoodItem food )
        {
            // Create invoice
            ...
            return Ok("Invoice Created");
        }    
    ```

    >Note: The `Topic` attribute is used to register the subscriber with the pub/sub component. `food-items` is the topic name and `food-pubsub` is the name of the pub/sub component.

- To publish an item use:

    ```
    POST http://localhost:5010/v1.0/publish/food-pubsub/food-items HTTP/1.1
    content-type: application/json

    {
        "id": 12,
        "name": "Pad Kra Pao",
        "price": 12.0,
        "inStock": 9,
        "pictureUrl": null,
        "code": "kra"
    }
    ```

## Dapr Bindings

Dapr bindings are a way to declaratively connect your application to another service. Dapr bindings are event-driven and can be triggered by an event or run on a schedule. Dapr bindings are implemented as an output binding, an input binding, or a bidirectional binding.

