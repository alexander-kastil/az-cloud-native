using FoodApp;
using FoodApp.MailDaemon;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configuration
IConfiguration Configuration = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(Configuration);
// var cfg = Configuration.Get<AppConfig>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

// app.UseHttpsRedirection();

app.MapPost("/send", ([FromBody] Mail mail, IConfiguration cfg) =>
{
    var appConfig = cfg.Get<AppConfig>();
    if (appConfig != null) GraphHelper.SendMail(mail.subject, mail.text, new[] { mail.recipient }, appConfig.GraphCfg);
    return Results.Ok();
})
.WithName("SendMail")
.WithOpenApi();

app.Run();