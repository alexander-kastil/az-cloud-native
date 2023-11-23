using FoodApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();
builder.AddApplicationInsights();
builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
builder.Services.AddDaprClient();
builder.AddEndpointsApiExplorer();
builder.AddNoCors();
builder.Services.AddControllers();

var app = builder.Build();
app.UseSwaggerUI();
app.UseNoCors();
app.UseDaprPubSub();
app.MapControllers();
app.Run();