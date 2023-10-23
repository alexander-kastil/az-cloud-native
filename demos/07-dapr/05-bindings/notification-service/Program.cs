using Dapr;
using Dapr.Client;
using FoodApp;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    Console.WriteLine("Hello Service Bus: " + req.Data);
    var metadata = new Dictionary<string, string>
    {
        { "toNumber", "<omitted>" }
    };
    var msg = $"Dear customer, your order with {req.Data.OrderId} was paid";
    try
    {
        await client.InvokeBindingAsync<string>(twilloBindingName, "create", msg);
    }
    catch (System.Exception ex)
    {        
        Console.WriteLine(ex.InnerException.Message);
    }
})

.WithName("ServiceBusTrigger")
.WithOpenApi();


app.Run();