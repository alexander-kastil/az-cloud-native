using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
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
            PaymentRequest paymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentRequest>(evt.Data.ToString());
            if (paymentRequest != null)
            {
                // When using transactional Outbox pattern, we need to add the payment to the database
                // We then could use Cosmos Change feed to execute the payment against our dapr bank service
                // PaymentTransaction payment = new PaymentTransaction()
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     CustomerId = evt.CustomerId,
                //     OrderId = paymentRequest.OrderId,
                //     PaymentInfo = paymentRequest.PaymentInfo,
                //     Amount = paymentRequest.Amount,
                //     Status = "Pending"
                // };            
                // await this.payment.AddPaymentAsync(payment);

                // To keep things simple we will just execute the payment against our dapr bank service
                // Make sure to created the bank account with the same account number 
                var usersBank = ActorProxy.Create<IBankActor>(new ActorId(paymentRequest.PaymentInfo.AccountNumber), "BankActor");
                // In a more realistic scenario we would need to check if the payment was successful - at the moment we just assume it was
                await usersBank.Withdraw(new WithdrawRequest() { Amount = paymentRequest.Amount });
                // Now we could issue a payment response just like we did in the previous lab
            }
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