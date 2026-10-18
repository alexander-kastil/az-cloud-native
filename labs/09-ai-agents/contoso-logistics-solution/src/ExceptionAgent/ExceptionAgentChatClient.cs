using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

public static class ExceptionAgentChatClient
{
    public static IHostApplicationBuilder AddExceptionAgentChatClient(this IHostApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["Foundry:Endpoint"]!;
        var deployment = builder.Configuration["Foundry:ChatDeployment"]!;

        builder.Services.AddChatClient(sp =>
            new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
                .GetChatClient(deployment)
                .AsIChatClient()
                .AsBuilder()
                .UseLogging(sp.GetRequiredService<ILoggerFactory>())
                .UseOpenTelemetry()
                .Build(sp));

        return builder;
    }
}
