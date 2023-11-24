using FoodApp;
var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();
builder.AddApplicationInsights();
builder.AddEndpointsApiExplorer();
builder.AddNoCors();

var app = builder.Build();
app.UseSwaggerUI();
app.UseNoCors();

app.MapPost("/upload-image", () =>
{
    
})
.WithName("Upload Image")
.WithOpenApi();

app.Run();