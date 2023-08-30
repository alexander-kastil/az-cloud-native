using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Integrations
{
    public class getValue
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public getValue(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<getValue>();
            _configuration = configuration;
        }

        [Function("getValue")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var queryDictionary = QueryHelpers.ParseQuery(req.Url.Query);
            if(queryDictionary["paramName"].Count > 0){
                var param = queryDictionary["paramName"][0];
                var value = _configuration[param];
                response.WriteString($"{param} has the following Value: {value}");
            }            
            
            return response;
        }
    }
}
