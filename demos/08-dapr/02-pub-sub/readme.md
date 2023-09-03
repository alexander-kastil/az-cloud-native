# Dapr Service Invocation, Pub / Sub & Bindings

Dapr pub/sub building block provides a platform-agnostic API framework to send and receive messages. The publisher services publish messages to a named topic. Your consumer services subscribe to a topic to consume messages:

- `pubsub-redis.yaml`:

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

## Publisher    

- Examine [FoodController.cs](../00-app/food-api-dapr/Controllers/FoodController.cs) 

    ```c#
    [Dapr.Topic("food-pubsub", "food-items")]
    [HttpPost("AddFoodPubSub")]
    public async Task<IActionResult> AddFood([FromBody] FoodItem food)
    {
        logger.LogInformation("Started processing message with food name '{0}'", food.Name);
        var existing = ctx.Food.FirstOrDefault(f => f.ID == food.ID);
        if (existing != null)
        {
            ctx.Attach<FoodItem>(food); 
            ctx.Entry(food).State = EntityState.Modified;
        }
        else
        {
            ctx.Food.Add(food);
            logger.LogInformation("Food with ID '{0}' does not exist. Adding it", food.ID);
        }
        await ctx.SaveChangesAsync();
        return Ok();
    }
    ```

     >Note: The `[Dapr.Topic]-annotation` is used to register pub/sub. `food-pubsub` is the name of the pub/sub component and `food-items` is the topic name.

- Run the api with Dapr and add the pub/sub component from the components folder:

    ```bash
    dapr run --app-id food-api-dapr --app-port 5000 --dapr-http-port 5010 --resources-path ../components -- dotnet run
    ```

    >Note: The `--resources-path` parameter is used to specify the location of the components folder. It adds all the components of the folder to the app.

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

## Subscriber

- Examine [Program.cs](../00-app/food-ui-dapr/Program.cs) of the subscriber and notice the following code:

    ```c#
    builder.Services.AddControllers().AddDapr();
    ...
    app.UseCloudEvents();
    ...
    app.MapSubscribeHandler();    
    ```

- `AddDapr()` registers the necessary services to integrate Dapr into the MVC pipeline. It also registers a `DaprClient` instance into the dependency injection container. 
- `UseCloudEvents()` adds CloudEvents middleware into the ASP.NET Core middleware pipeline. This middleware will unwrap requests that use the CloudEvents structured format, so the receiving method can read the event payload directly.
- `MapSubscribeHandler()` registers a route handler for the `dapr/subscribe` endpoint. This endpoint is used by Dapr to register the subscriber with the pub/sub component. The route handler will read the topic name from the request and register the subscriber with the pub/sub component.    

- Examine the current state of [HomeController.cs](../00-app/food-ui-dapr/Controllers/HomeController.cs) and notice that it is using direct service invocation to get the food items:
    
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

- Run the UI and test the implementation:

    ```bash
    cd food-dapr-fronted
    dapr run --app-id food-fronted --app-port 5002 --dapr-http-port 5011 dotnet run
    ```

- It should return the following result:

    ![food-app](_images/food-app.png)
