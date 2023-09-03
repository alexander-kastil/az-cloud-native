using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public FoodController(FoodDBContext context, ILogger<FoodController> ILogger)
        {
            ctx = context;
            logger = ILogger;
        }

        // http://localhost:PORT/food
        [HttpGet()]
        public IEnumerable<FoodItem> GetFood()
        {
            return ctx.Food.ToArray();
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddFood([FromBody] FoodItem food)
        {
            logger.LogInformation("Started processing message with food name '{0}'", food.Name);
            var existing = ctx.Food.FirstOrDefault(f => f.ID == food.ID);
            if (existing != null)
            {
                ctx.Attach(food); 
                ctx.Entry(food).State = EntityState.Modified;
            }
            else
            {
                ctx.Food.Add(food);
                logger.LogInformation("Food with ID '{0}' does not exist. Adding it", food.ID);
            }
            await ctx.SaveChangesAsync();
            return Ok();
        }
    }
}