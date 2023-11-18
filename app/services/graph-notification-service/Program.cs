using FoodApp;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.AddConfig();
builder.AddEndpointsApiExplorer(cfg.Title);

var app = builder.Build();
app.UseSwaggerUI(cfg.Title);

app.MapPost("/send", ([FromBody] Mail mail, IConfiguration cfg) =>
{
    var appConfig = cfg.Get<AppConfig>();
    if (appConfig != null) GraphHelper.SendMail(mail.subject, mail.text, new[] { mail.recipient }, appConfig.GraphCfg);
    return Results.Ok();
})
.WithName("SendMail")
.WithOpenApi();

app.Run();