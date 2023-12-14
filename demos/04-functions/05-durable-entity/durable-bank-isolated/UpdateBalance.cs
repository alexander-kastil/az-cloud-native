using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FoodApp
{
    public class UpdateBalance
    {
        private readonly ILogger _logger;

        public UpdateBalance(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UpdateBalance>();
        }

         // Add this using directive

                [Function("UpdateBalance")]
                public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", "post", Route = "bankAccount/updateBalance/{entityKey}/{amount}")] 
                    HttpRequestData req,  [DurableClient] DurableTaskClient client, string entityKey, string amount)
                {
                    _logger.LogInformation("C# HTTP trigger function processed a request.");

                    var response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                    response.WriteString("Welcome to Azure Functions!");

                    return response;
                }
    }
}
