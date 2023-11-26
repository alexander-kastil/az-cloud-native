using FoodApp;

var builder = WebApplication.CreateBuilder(args);
AppConfig cfg = builder.AddConfig();
builder.AddApplicationInsights();

// Service Bus
var eb = new EventBus(cfg.ServiceBus.ConnectionString, cfg.ServiceBus.QueueName);
builder.Services.AddSingleton<EventBus>(eb);

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
// Add Repositories for OrderAggregates and OrderEvents
OrderAggregates orderAggregates = new OrderAggregates(cfg.CosmosDB.GetConnectionString(), cfg.CosmosDB.DBName, cfg.CosmosDB.OrderAggregatesContainer);
builder.Services.AddSingleton<IOrderAggregates>(orderAggregates);
OrderEventsStore orderEventsStore = new OrderEventsStore(cfg.CosmosDB.GetConnectionString(), cfg.CosmosDB.DBName, cfg.CosmosDB.OrderEventsContainer);
builder.Services.AddSingleton<IOrderEventsStore>(orderEventsStore);

builder.AddEndpointsApiExplorer(cfg.Title); 
builder.AddNoCors();
builder.Services.AddControllers();
var app = builder.Build();
app.UseSwaggerUI(cfg.Title);
app.UseNoCors();
app.MapControllers();
app.Run();