using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoodDapr;

public class HomeController : Controller
{
    
    private readonly string BACKEND_NAME;
    private readonly string BACKEND_PORT;
    
    private readonly DaprClient client;

    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> ILogger, DaprClient daprClient)
    {
        logger = ILogger;
        client = daprClient;
        BACKEND_NAME = Environment.GetEnvironmentVariable("BACKEND_NAME") ?? "food-backend";
        BACKEND_PORT = Environment.GetEnvironmentVariable("BACKEND_DAPR_HTTP_PORT") ?? "5010";
    }

    public async Task<IActionResult> Index()
    {
        HttpClient client = new HttpClient();
        var re = await client.GetAsync($"http://localhost:{BACKEND_PORT}/v1.0/invoke/{BACKEND_NAME}/method/food");
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
