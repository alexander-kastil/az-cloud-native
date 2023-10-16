using System.IO;
using System.Net;
using System.Net.Http;
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
    public class GetFoodById
    {
        private readonly FoodDBContext _context;
        private readonly ILogger<GetFoodById> _logger;

        public GetFoodById(ILogger<GetFoodById> log, FoodDBContext context)
        {
            _logger = log;
            _context = context;
        }

        [FunctionName("GetFoodById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The Id of the Item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(int), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getById/{id}")] 
            HttpRequest req, int id)
        {
            _logger.LogInformation("GetFoodById executed");                  
            var item = await _context.Food.FindAsync(id);
            return new OkObjectResult(item);
        }
    }
}

