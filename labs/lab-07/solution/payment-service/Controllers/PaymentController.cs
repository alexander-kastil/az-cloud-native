using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        AILogger logger;
        IPaymentRepository payment;

        public PaymentController(IPaymentRepository repository, AILogger aILogger)
        {
            logger = aILogger;
            payment = repository;
        }

        // http://localhost:PORT/payment/create
        [HttpPost()]
        [Route("create")]
        [Dapr.Topic("food-pubsub", "payment-requested")]
        public async Task AddPayment(OrderEvent evt)        
        {
            Console.WriteLine($"Received payment request for order: {evt.OrderId}", evt);
            // PaymentTransaction payment = new PaymentTransaction();
            // await this.payment.AddPaymentAsync(payment);
        }

        // http://localhost:PORT/payment/getAll
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<PaymentTransaction>> GetAllPaymentsAsync()
        {
            return await payment.GetAllPaymentsAsync();
        }

        // http://localhost:PORT/orders/getById/{id}/{customerId
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<PaymentTransaction> GetPaymentByIdAsync(string id, string customerId)
        {
            return await payment.GetPaymentByIdAsync(id, customerId);
        }

        // http://localhost:PORT/orders/update
        [HttpPut()]
        [Route("update")]
        public async Task<IActionResult> UpdatePaymentAsync(PaymentTransaction payment)
        {
            await this.payment.UpdatePaymentAsync(payment.Id, payment);
            return Ok();
        }
    }
}
