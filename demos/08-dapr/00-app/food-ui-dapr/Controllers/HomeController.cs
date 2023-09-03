using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoodDapr;

public class HomeController : Controller
{
    const string storeName = "statestore";
    const string key = "counter";

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var daprClient = new DaprClientBuilder().Build();
        var counter = await daprClient.GetStateAsync<int>(storeName, key);
        counter++;
        await daprClient.SaveStateAsync(storeName, key, counter);
        ViewBag.Counter = counter;

        var port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");

        HttpClient client = new HttpClient();
        var re = await client.GetAsync($"http://localhost:{port}/v1.0/invoke/food-api-dapr/method/food");
        var text = await re.Content.ReadAsStringAsync();
        ViewBag.Text = text + "," + re.StatusCode + ",";
        ViewBag.Food = JsonSerializer.Deserialize<List<FoodItem>>(text);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
