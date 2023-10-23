using FoodApp.DataGenerator;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Dapr
builder.Services.AddDaprClient();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Food Payments Dapr", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Food Payments Dapr");
    c.RoutePrefix = string.Empty;
});

// app.UseHttpsRedirection();

app.MapPost("/execPayment", (PaymentRequest request) =>
{ 
    var resp = new PaymentResponse
    {
        OrderId = request.OrderId,
        CustomerId = request.CustomerId,
        CustomerName = request.CustomerName,
        PaymentAmount = request.PaymentAmount,
        PaymentStatus = "Success",
        PaymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    };
    return resp;
})
.WithName("ExecutePayment")
.WithOpenApi();

app.Run();