
public static class FoodBuilder
{
    public static void AddConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
    }

    public static void AddEndpointsApiExplorer(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
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