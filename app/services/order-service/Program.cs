using FoodApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();
builder.AddApplicationInsights();

builder.Services.AddDaprClient();
builder.Services.AddSingleton<IDaprEventBus, DaprEventBus>();

// Add OrderAggregates and OrderEvents
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<IOrderAggregates, OrderAggregates>();
builder.Services.AddSingleton<IOrderEventsStore, OrderEventsStore>();

builder.AddEndpointsApiExplorer();
builder.AddNoCors();
builder.Services.AddControllers();
var app = builder.Build();
app.UseSwaggerUI();
app.UseNoCors();
app.UseDaprPubSub();
app.MapControllers();
app.Run();