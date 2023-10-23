using Dapr;
using FoodApp.DataGenerator;
using Microsoft.OpenApi.Models;

var cronBindingName = "cron";
var paymentBindingName = "execPayment";
var sqlBindingName = "sqldb";

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

if (app.Environment.IsDevelopment()) {app.UseDeveloperExceptionPage();}


app.MapPost("/"+ cronBindingName, () =>
{
    Console.WriteLine("Hello World at" + DateTime.Now);
})
.WithName("CronTrigger")
.WithOpenApi();

app.MapPost("/"+ paymentBindingName, (CloudEvent<PaymentRequest> req) =>
{
    Console.WriteLine("Hello Service Bus" + DateTime.Now);
})
.WithName("ServiceBusTrigger")
.WithOpenApi();


app.Run();