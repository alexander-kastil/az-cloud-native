using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FoodDapr
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpPost]
        [Dapr.Topic("food-pubsub", "food-items")]
        public ActionResult CreateInvoice(FoodItem food )
        {
            Console.WriteLine($"Received food item {food.Name} with price {food.Price}");
            Console.WriteLine("Creating invoice");
            return Ok("Invoice Created");
        }    
    }
}