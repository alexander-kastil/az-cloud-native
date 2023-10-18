using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
IConfigurationRoot configuration = builder.Build();

var connectionString = configuration["storageAcctConStr"];
var q = configuration["queueName"];
var items = int.Parse(configuration["items"]);
QueueClient queue = new QueueClient(connectionString, q);

for (int i = 0; i < items; i++)
{
    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("Hello World - round " + i.ToString());
    var value = System.Convert.ToBase64String(plainTextBytes);
    await InsertMessageAsync(queue, value);
    Console.WriteLine($"Sent: {value}");
}

static async Task InsertMessageAsync(QueueClient q, string msg)
{
    if (null != await q.CreateIfNotExistsAsync())
    {
        Console.WriteLine("The queue was created.");
    }

    await q.SendMessageAsync(msg);
}