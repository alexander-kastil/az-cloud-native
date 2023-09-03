# Publish & subscribe

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

- [FoodController.cs](../00-app/food-api-dapr/Controllers/FoodController.cs) 

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

- Run the api with Dapr and add the pub/sub component from the components folder:

    ```bash
    dapr run --app-id food-api-dapr --app-port 5000 --dapr-http-port 5010 --resources-path ../components -- dotnet run
    ```

    >Note: The `--resources-path` parameter is used to specify the location of the components folder. It adds all the components of the folder to the app.

- To post an item use:

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

    >Note: `food-pubsub` is the name of the pub/sub component and `food-items` is the topic name.