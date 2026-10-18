# Contoso Logistics Events

## Events

| Event | Journey stage | Kind | Style | Key fields | Emitted by |
| --- | --- | --- | --- | --- | --- |
| ShipmentBooked | booking | integration | state transfer | shipment ID, tracking number, sender, recipient address, parcel size and weight, service level, booking time | Shipment |
| CapacityReserved | booking | integration | notification | shipment ID, tracking number, reservation ID, reserved time | Route |
| PaymentCaptured | billing | integration | notification | shipment ID, tracking number, payment ID, amount, currency, captured time | Payment |
| LabelPrinted | pickup | domain | notification | shipment ID, tracking number, label ID, printed time | Label |
| ParcelScanned | transit | integration | notification | parcel ID, shipment ID, tracking number, scan type (PickedUp, AtHub or OutForDelivery), scan location, scan time | parcel scanner |
| VanPositionReported | transit | integration | state transfer | van ID, latitude and longitude, speed, reported time | van telematics |
| DeliveryAttempted | delivery | integration | notification | shipment ID, tracking number, attempt number, outcome, attempt time | Delivery |
| DeliveryDelayed | delivery | integration | notification | shipment ID, tracking number, delay reason, new estimated arrival time | Delivery |
| ParcelDelivered | delivery | integration | state transfer | shipment ID, tracking number, final status, recipient name, delivered time | Delivery |
| ProofOfDeliveryUploaded | delivery | domain | notification | shipment ID, tracking number, proof document reference, uploaded time | Delivery |
| RerouteRequested | delivery | integration | notification | shipment ID, tracking number, requested new address, reason, requester | ops-console |
| RerouteApproved | delivery | integration | state transfer | shipment ID, tracking number, new address, new route, new estimated arrival time | Route |
| PaymentRefunded | billing | integration | notification | shipment ID, tracking number, payment ID, refunded amount, refund reason | Payment |

## Commands

| Command | Sent by | Accepted by | Emits |
| --- | --- | --- | --- |
| BookShipment | shipping-api | Shipment | ShipmentBooked |
| ReserveCapacity | shipping-api | Route | CapacityReserved |
| CapturePayment | billing-service | Payment | PaymentCaptured |
| PrintLabel | label-service | Label | LabelPrinted |
| AssignRoute | routing-service | Route | None |
| RerouteParcel | exception-agent | Route | RerouteApproved |
| NotifyCustomer | tracking-service | CustomerNotification | None |

## Aggregates

| Aggregate | Owns | Accepts commands | Emits events |
| --- | --- | --- | --- |
| Shipment | booking details, tracking number, sender and recipient, service level | BookShipment | ShipmentBooked |
| Route | route plan, van assignment, remaining van capacity | ReserveCapacity, AssignRoute, RerouteParcel | CapacityReserved, RerouteApproved |
| Payment | amount, currency, captured and refunded state | CapturePayment | PaymentCaptured, PaymentRefunded |
| Label | label ID, printed state | PrintLabel | LabelPrinted |
| Delivery | delivery attempts, delay status, proof of delivery, final status | None | DeliveryAttempted, DeliveryDelayed, ParcelDelivered, ProofOfDeliveryUploaded |
| CustomerNotification | message content, channel, send status | NotifyCustomer | None |
