# Services 

To make local development and debug easier use the following ports reference for the services:

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port| Stack   |
| -------                   | --------- | ---------- | --------- | -------------        | -----      | ------- |
| Catalog Service           | 5001      | 5021       | 5011      | catalog-service      | 5051       | .NET 6  |
| Order Service             | 5002      | 5022       | 5012      | order-service        | 5052       | .NET 6  |
| Payment Service           | 5004      | 5024       | 5014      | payment-service      | 5054       | .NET 6  |
| Bank Actor Service        | 5005      | 5025       | 3500      | bank-actor           | 5055       | .NET 8  |
| Cooking Service           | 5006      | 5026       | 5016      | cooking-service      | 5056       | .NET 8  |
| Delivery Service          | 5007      | 5027       | 5017      | deliver-service      | 5057       | .NET 8  |
| Notification Service      | 5008      | 5028       | 5018      | notification-service | 5058       | .NET 8  |
| Image Service             | 5009      | 5029       | 5019      | image-service        | 5059       | .NET 8  |

 
| Azure Functions                 | Http Port | Process Model|
| -------                         | --------- | ---------- | 
| Order Event Processor Function  | 7073      |            |	
| Payment Service Function        | 7074      |            | 
| Cooking Dashboard Function      | 7076      |            |
| Optimizer Function              | 7077      |            |
| Invoices Job Function           | 7078      |            |