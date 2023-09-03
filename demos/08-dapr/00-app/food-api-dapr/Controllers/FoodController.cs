using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FoodDapr
{
    [Route("[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        FoodDBContext ctx;
        private readonly ILogger logger;

        public FoodController(FoodDBContext context, IConfiguration config, ILogger<FoodController> ilogger)
        {
            ctx = context;
            logger = ilogger;
        }

        // http://localhost:PORT/food
        [HttpGet()]
        public IEnumerable<FoodItem> GetFood()
        {
            return ctx.Food.ToArray();
        }

        [Dapr.Topic("foodpubsub", "addfood")]
        [HttpPost("AddFoodPubSub")]
        public async Task<IActionResult> AddFood([FromBody] FoodItem food)
        {
            logger.LogInformation("Started processing message with food name '{0}'", food.Name);

            ctx.Food.Add(food);
            await ctx.SaveChangesAsync();
            return Ok();
        }

        // [HttpPost()]
        // public ActionResult<FoodItem> AddFood(FoodItem food)
        // {
        //     ctx.Food.Add(food);
        //     ctx.SaveChanges();
        //     return food;
        // }
    }
}