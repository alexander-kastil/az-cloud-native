using FoodApp;
var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();
builder.AddApplicationInsights();
builder.AddEndpointsApiExplorer(cfg.Title);
builder.AddNoCors();

var app = builder.Build();
app.UseSwaggerUI(cfg.Title);
app.UseNoCors();

app.MapGet("/weatherforecast", () =>
{
    
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();