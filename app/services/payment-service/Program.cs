using FoodApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();
builder.AddApplicationInsights();
// Add cosmos db service
// PaymentRepository cosmosDbService = new PaymentRepository(cfg.CosmosDB.ConnectionString, cfg.CosmosDB.DBName, cfg.CosmosDB.Container);
builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
builder.Services.AddDaprClient();
builder.AddEndpointsApiExplorer(cfg.Title);
builder.AddNoCors();
builder.Services.AddControllers();

var app = builder.Build();
app.UseSwaggerUI(cfg.Title);
app.UseNoCors();
app.UseDaprPubSub();
app.MapControllers();
app.Run();