using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;

namespace FoodDapr
{
    [Route("[controller]")]
    [ApiController]
    public class CountController : ControllerBase
    {        
        const string storeName = "statestore";
        const string key = "counter";

        [HttpGet("getCount")]
        public async Task<int> Get()
        {
            var daprClient = new DaprClientBuilder().Build();
            var counter = await daprClient.GetStateAsync<int>(storeName, key);
            await daprClient.SaveStateAsync(storeName, key, counter + 1);
            return counter;
        }

        // [HttpGet("{id}")]
        // public ActionResult<string> Get(int id)
        // {
        //     return "value";
        // }

        // [HttpPost]
        // public void Post([FromBody] string value)
        // {
        // }

        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }

        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}