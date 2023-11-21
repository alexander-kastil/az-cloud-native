using FoodApp;

var builder = WebApplication.CreateBuilder(args);
AppConfig cfg = builder.AddConfig() as AppConfig;
builder.AddApplicationInsights();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BankService>();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<BankActor>();
});

var app = builder.Build();

app.UseRouting();

app.MapActorsHandlers();

app.Run();