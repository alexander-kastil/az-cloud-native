using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace food_invoices_dapr
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        [Dapr.Topic("food-pubsub", "food-items")]
        public ActionResult CreateInvoice()
        {
            return Ok("Invoice Created");
        }    
    }
}