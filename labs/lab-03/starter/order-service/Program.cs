using FoodApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
AppConfig cfg = builder.AddConfig() as AppConfig;
builder.AddApplicationInsights();

builder.AddEndpointsApiExplorer(cfg.Title);
builder.AddNoCors();
builder.Services.AddControllers();
var app = builder.Build();
app.UseSwaggerUI(cfg.Title);
app.UseNoCors();
app.MapControllers();
app.Run();