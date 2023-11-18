using FoodApp;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();

builder.Services.AddSingleton<GraphHelper>();
builder.Services.AddDaprClient();

builder.AddEndpointsApiExplorer(cfg.Title);
builder.AddApplicationInsights();

var app = builder.Build();
app.UseSwaggerUI(cfg.Title);

app.MapPost("/send", [Dapr.Topic("food-pubsub", "notification-requests")] ([FromBody]Mail mail, GraphHelper graph) =>
{    
    graph.SendMail(mail.Subject, mail.Text, new[] { mail.Recipient }   );
    return Results.Ok();
})
.WithName("SendMail")
.WithOpenApi();

app.MapPost("/pubsub-test", [Dapr.Topic("food-pubsub", "pubsub-test")] ([FromBody]TestMessage msg) =>
{    
    Console.WriteLine(msg);
    return Results.Ok();
})
.WithName("test")
.WithOpenApi();


app.UseDaprPubSub();

app.Run();