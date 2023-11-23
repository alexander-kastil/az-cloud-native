using FoodApp;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();

builder.Services.AddDaprClient();
builder.Services.AddSingleton<ICookingRepository, CookingRepository>();

builder.AddEndpointsApiExplorer();
builder.AddApplicationInsights();

var app = builder.Build();
app.UseSwaggerUI();

app.MapGet("/cooking-request", [Dapr.Topic("food-pubsub", "notification-requests")] async (OrderEvent @event, CookingRepository rep) =>
{
    var order = @event.Data as Order;
    await rep.AddOrderAsync(order);
    return Results.Ok();
})
.WithName("cooking-request")
.WithOpenApi();

app.UseDaprPubSub();

app.Run();