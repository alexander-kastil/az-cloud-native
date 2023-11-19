using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using Dapr.Client;
using IBankActorInterface;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        AILogger logger;
        IPaymentRepository payment;
        DaprClient dapr;

        public PaymentController(IPaymentRepository repository,  DaprClient daprClient, AILogger aILogger)
        {
            logger = aILogger;
            payment = repository;
            dapr = daprClient;
        }

        // http://localhost:PORT/payment/create
        [HttpPost()]
        [Route("create")]
        [Dapr.Topic("food-pubsub", "payment-requests-topic")]
        public async Task ExecutePayment(OrderEvent evt)
        {
            await payment.ExecutePayment(evt);
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