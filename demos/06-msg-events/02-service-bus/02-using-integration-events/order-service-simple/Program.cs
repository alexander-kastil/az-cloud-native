using FoodApp.Common;
using FoodApp.OrderService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container -> Configure Services in Startup.cs
var cfg = builder.Configuration.Get<OrdersConfig>();

// Service Bus
var proxy = new ServiceBusProxy(cfg.App.ServiceBus.ConnectionString, cfg.App.ServiceBus.QueueName);
builder.Services.AddSingleton<ServiceBusProxy>(proxy);

// Entity Framework
builder.Services.AddDbContext<FoodOrderDBContext>(opts => opts.UseSqlite(cfg.App.ConnectionStrings.SQLiteDBConnection));
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders Api Simple", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline -> Configure in Startup.cs
// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders Api Simple");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
