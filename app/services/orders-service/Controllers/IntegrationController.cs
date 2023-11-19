using Microsoft.AspNetCore.Mvc;
using MediatR;
using Newtonsoft.Json;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly ISender sender;
        private readonly IDaprEventBus eb;
        AILogger logger;

        public IntegrationController(ISender sender,IDaprEventBus eventBus, AILogger aiLogger)
        {
            this.sender = sender;
            this.logger = aiLogger;
            this.eb = eventBus;
        }

        [HttpPost()]
        [Route("handle-payment-response")]
        [Dapr.Topic("food-pubsub", "payment-response-topic")]
        public async Task HandlePaymentResponse(PaymentResponse response)
        {
            if (response.Status == "success")
            {                
                this.eb.Publish(new OrderEvent
                {
                    // OrderId = response.OrderId,
                    // CustomerId = response.CustomerId,
                    // EventType = "cooking-requested",
                    // Data = JsonConvert.SerializeObject(new CookingRequest
                    // {
                    //     OrderId = response.OrderId,
                    //     CustomerId = response.CustomerId
                    // })
                });
            }
            else
            {
                // Handle Payment Failure using MediatR
            }
        }

        [HttpPost()]
        [Route("handle-cooking-response")]
        [Dapr.Topic("food-pubsub", "cooking-response-topic")]
        public async Task HandleCookingResponse(PaymentResponse response)
        {
            if (response.Status == "success")
            {                
                this.eb.Publish(new OrderEvent
                {
                    // OrderId = response.OrderId,
                    // CustomerId = response.CustomerId,
                    // EventType = "delivery-requested",
                    // Data = JsonConvert.SerializeObject(new DeliveryRequest
                    // {
                    //     OrderId = response.OrderId,
                    //     CustomerId = response.CustomerId
                    // })
                });
            }
            else
            {
                // Handle Cooking Failure using MediatR
            }
        }

        [HttpPost()]
        [Route("handle-delivery-response")]
        [Dapr.Topic("food-pubsub", "delivery-response-topic")]
        public async Task HandleDeliveryResponse(PaymentResponse response)
        {
            if (response.Status == "success")
            {                
                
            }
            else
            {
                // Handle Delivery Failure using MediatR
            }
        }
    }
}