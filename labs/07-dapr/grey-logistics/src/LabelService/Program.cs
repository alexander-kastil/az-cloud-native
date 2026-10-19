using Azure.Core;
using Azure.Identity;
using LabelService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
builder.Services.AddSingleton(sp => new ParcelDeliveredPublisher(
    new Uri(builder.Configuration["Azure:EventGrid:TopicEndpoint"]!),
    sp.GetRequiredService<TokenCredential>()));
builder.Services.AddHostedService<ProofOfDeliveryWorker>();

builder.Build().Run();
