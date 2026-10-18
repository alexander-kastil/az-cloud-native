using TelemetryIngest;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
var signalR = builder.Services.AddSignalR();
var signalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];
if (!string.IsNullOrEmpty(signalRConnectionString))
{
    signalR.AddAzureSignalR(signalRConnectionString);
}
builder.Services.AddSingleton<ConnectedClients>();
builder.Services.AddHostedService<VanPositionProcessor>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(builder.Configuration["OpsConsole:Origin"]!)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseCors();
app.MapHub<VanPositionHub>("/hubs/van-positions");

app.Run();
