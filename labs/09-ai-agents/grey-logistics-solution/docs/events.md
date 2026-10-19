# Grey Logistics Events

## Events

| Event | Journey stage | Kind | Style | Key fields | Emitted by |
| --- | --- | --- | --- | --- | --- |
| ShipmentBooked | booking | integration | state transfer | shipment ID, tracking number, sender, recipient address, parcel size and weight, service level, booking time | Shipment in shipping-api (outbox relay) |
| CapacityReserved | booking | integration | notification | shipment ID, tracking number, reservation ID, reserved time | Route (design) |
| PaymentCaptured | billing | integration | notification | shipment ID, tracking number, payment ID, amount, currency, captured time | Payment (design) |
| LabelPrinted | pickup | domain | notification | shipment ID, tracking number, label ID, printed time | Label (design) |
| ParcelScanned | transit | integration | notification | parcel ID, shipment ID, tracking number, scan type (PickedUp, AtHub or OutForDelivery), scan location, scan time | parcel scanner (ScanBacklog in the lab) |
| VanPositionReported | transit | integration | state transfer | van ID, sequence, latitude and longitude, observed time | van telematics (VanSimulator in the lab) |
| DeliveryAttempted | delivery | integration | notification | shipment ID, tracking number, attempt number, outcome, attempt time | Delivery (design) |
| DeliveryDelayed | delivery | integration | notification | shipment ID, tracking number, delay reason, new estimated arrival time | booking-worker (DeliveryDeadlineOrchestrator) |
| ParcelDelivered | delivery | integration | state transfer | shipment ID, tracking number, final status, recipient name, delivered time | label-service (Event Grid) |
| ProofOfDeliveryUploaded | delivery | domain | notification | shipment ID, tracking number, proof document reference, uploaded time | Delivery (design) |
| RerouteRequested | delivery | integration | notification | shipment ID, tracking number, requested new address, reason, requester | exception-agent |
| RerouteApproved | delivery | integration | state transfer | shipment ID, tracking number, new address, new route, new estimated arrival time | HTTP caller of booking-worker (reroute-approval) |
| PaymentRefunded | billing | integration | notification | shipment ID, tracking number, payment ID, refunded amount, refund reason | Payment (design) |

## Commands

| Command | Sent by | Accepted by | Emits |
| --- | --- | --- | --- |
| BookShipment | Customer over HTTP (POST /shipments) | Shipment in shipping-api | ShipmentBooked |
| ReserveCapacity | booking-worker saga (design) | Route in routing-service (/reserve-capacity) | CapacityReserved (design) |
| CapturePayment | booking-worker saga (design) | Payment (design) | PaymentCaptured (design) |
| PrintLabel | booking-worker saga (design) | Label (design) | LabelPrinted (design) |
| AssignRoute | routing-service (design) | Route (design) | None |
| RerouteParcel | booking-worker saga (activity records only) | Route (design) | RerouteApproved (HTTP, design) |
| NotifyCustomer | tracking-service (design) | CustomerNotification (design) | None |

## Aggregates

| Aggregate | Owns | Accepts commands | Emits events |
| --- | --- | --- | --- |
| Shipment | booking details, tracking number, sender and recipient, service level | BookShipment | ShipmentBooked |
| Route (design) | route plan, van assignment, remaining van capacity | ReserveCapacity, AssignRoute, RerouteParcel | CapacityReserved, RerouteApproved |
| Payment (design) | amount, currency, captured and refunded state | CapturePayment | PaymentCaptured, PaymentRefunded |
| Label (design) | label ID, printed state | PrintLabel | LabelPrinted |
| Delivery (design) | delivery attempts, delay status, proof of delivery, final status | None | DeliveryAttempted, DeliveryDelayed, ParcelDelivered, ProofOfDeliveryUploaded |
| CustomerNotification (design) | message content, channel, send status | NotifyCustomer | None |
