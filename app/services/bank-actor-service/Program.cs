using FoodApp;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();
builder.AddApplicationInsights();
builder.Services.AddSingleton<BankService>();
builder.AddEndpointsApiExplorer();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<BankActor>();
});
builder.Services.AddControllers();

var app = builder.Build();
app.UseSwaggerUI();
app.UseRouting();
app.MapActorsHandlers();
app.MapControllers();
app.Run();