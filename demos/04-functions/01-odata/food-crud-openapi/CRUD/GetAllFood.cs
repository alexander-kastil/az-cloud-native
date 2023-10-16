using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FoodApp
{
    public class GetAllFood
    {
        private readonly FoodDBContext _context;

        private readonly ILogger<GetAllFood> _logger;

        public GetAllFood(ILogger<GetAllFood> log, FoodDBContext context)        
        {
            _logger = log;
            _context = context;
        }

        [FunctionName("GetAllFood")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetAllFood executed");
            var food =  await _context.Food.ToListAsync<FoodItem>();
            return new OkObjectResult(food);
        }
    }
}

