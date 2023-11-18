using FoodApp;
using Microsoft.OpenApi.Models;

public static class AppBuilder
{
    public static AppConfig AddConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        return builder.Configuration.Get<AppConfig>();
    }

    public static void AddEndpointsApiExplorer(this WebApplicationBuilder builder, string title)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
        });
    }

    public static void UseSwaggerUI(this WebApplication app, string title)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", title);
            c.RoutePrefix = string.Empty;
        });
    }
}