using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FoodApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration Configuration = builder.Configuration;
builder.Services.AddSingleton(Configuration);

string conString = Configuration["SQLiteDBConnection"];
builder.Services.AddDbContext<FoodDBContext>(options => options.UseSqlite(conString));

// Dapr
builder.Services.AddDaprClient();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hello-Dapr", Version = "v1" });
});

// Cors
builder.Services.AddCors(o => o.AddPolicy("nocors", builder =>
{
    builder
        .SetIsOriginAllowed(host => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hello-Dapr");
    c.RoutePrefix = string.Empty;
});

//Cors and Routing
app.UseCors("nocors");

app.UseHttpsRedirection();

app.UseAuthorization();

// Dapr Subscribe Handler
app.MapSubscribeHandler();

app.MapControllers();

app.Run();
