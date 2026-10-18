using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScanProcessor;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddAzureCosmosClient("cosmos");
builder.Services.AddSingleton(sp => new ParcelScanStore(sp.GetRequiredService<CosmosClient>().GetContainer("logistics", "tracking")));

builder.Build().Run();
