using FoodApp;
var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();
builder.AddApplicationInsights();
builder.AddEndpointsApiExplorer(cfg.Title);
builder.AddNoCors();

var app = builder.Build();
app.UseSwaggerUI(cfg.Title);
app.UseNoCors();

app.MapPost("/upload-image", () =>
{
    
})
.WithName("Upload Image")
.WithOpenApi();

app.Run();