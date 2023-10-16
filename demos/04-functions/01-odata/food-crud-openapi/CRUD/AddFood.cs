using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FoodApp
{
    public class AddFood
    {
        private readonly FoodDBContext _context;
        private readonly ILogger<AddFood> _logger;

        public AddFood(ILogger<AddFood> log, FoodDBContext context)
        {
            _logger = log;
            _context = context;
        }

        [FunctionName("AddFood")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "Food", In = ParameterLocation.Query, Required = true, Type = typeof(FoodItem), Description = "The item to add")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("AddFood executed");
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<FoodItem>(content);
            _context.Food.Add(item);
            await _context.SaveChangesAsync();    
            return new OkObjectResult(true);
        }
    }
}

