using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dapr.Client;

namespace FoodDapr;

public class HomeController : Controller
{    
    private readonly string BACKEND_NAME;
    private readonly string BACKEND_PORT;
    private readonly IConfiguration cfg;    
    private readonly DaprClient client;
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> ILogger, DaprClient daprClient, IConfiguration configuration)
    {
        logger = ILogger;
        client = daprClient;
        cfg = configuration;
        BACKEND_NAME = cfg.GetValue<string>("BACKEND_NAME") ?? "food-api";
        BACKEND_PORT = cfg.GetValue<string>("BACKEND_DAPR_HTTP_PORT") ?? "5010";
    }

    public async Task<IActionResult> Index()
    {
        HttpClient client = new HttpClient();
        var daprResponse = await client.GetAsync($"http://localhost:{BACKEND_PORT}/v1.0/invoke/{BACKEND_NAME}/method/food");
        var jsonFood = await daprResponse.Content.ReadAsStringAsync();
        ViewBag.Food =  JsonConvert.DeserializeObject<List<FoodItem>>(jsonFood);;
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
