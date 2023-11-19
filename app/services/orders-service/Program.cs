using FoodApp;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();
builder.AddApplicationInsights();

builder.Services.AddDaprClient();
builder.Services.AddSingleton<IDaprEventBus, DaprEventBus>();

// Add OrderAggregates and OrderEvents
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

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
// app.UseAuthorization();
app.UseDaprPubSub();
app.MapControllers();
app.Run();