using FoodApp;
using FoodApp.MailDaemon;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfig();
builder.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseSwaggerUI("Notification Service");
app.UseDeveloperExceptionPage();

app.MapPost("/send", ([FromBody] Mail mail, IConfiguration cfg) =>
{
    var appConfig = cfg.Get<AppConfig>();
    if (appConfig != null) GraphHelper.SendMail(mail.subject, mail.text, new[] { mail.recipient }, appConfig.GraphCfg);
    return Results.Ok();
})
.WithName("SendMail")
.WithOpenApi();

app.Run();