using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConfigApi
{
    [Route("[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        FoodDBContext ctx;

        public FoodController(FoodDBContext context, IConfiguration config)
        {
            ctx = context;
        }

        // http://localhost:PORT/food
        [HttpGet()]
        public async Task<IEnumerable<FoodItem>> GetFood()
        {
            return await ctx.Food.ToArrayAsync();
        }

         // http://localhost:PORT/food/3
        [HttpGet("{id}")]
        public async Task<FoodItem> GetById(int id)
        {
             return await ctx.Food.FirstOrDefaultAsync(v => v.ID == id);
        }
    }
}