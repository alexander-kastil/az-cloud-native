using Dapr;
using Dapr.Client;
using FoodApp;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var cronBindingName = "cron";
var paymentBindingName = "execPayment";
var twilloBindingName = "sms-twilio";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }


app.MapPost("/" + cronBindingName, () =>
{
    Console.WriteLine("Hello World at" + DateTime.Now);
})
.WithName("CronTrigger")
.WithOpenApi();

app.MapPost("/" + paymentBindingName, async (CloudEvent<PaymentRequest> req) =>
{
    var payment = req.Data;
    var jsonPayment = JsonSerializer.Serialize(payment);
    using var client = new DaprClientBuilder().Build();
    Console.WriteLine("Received msg from Service Bus: " + req.Data);
    var msg = $"Dear customer, your order with {req.Data.OrderId} was paid";
    await client.InvokeBindingAsync<string>(twilloBindingName, "create", msg);    
})

.WithName("ServiceBusTrigger")
.WithOpenApi();


app.Run();