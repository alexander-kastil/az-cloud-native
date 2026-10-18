public class Shipment
{
    private Shipment(string shipmentId, int version, string status)
    {
        ShipmentId = shipmentId;
        Version = version;
        Status = status;
    }

    public string ShipmentId { get; }

    public int Version { get; }

    public string Status { get; }

    public static Shipment Empty(string shipmentId) => new(shipmentId, 0, "None");

    public Shipment Apply(ShipmentDocument document)
    {
        var status = document.eventType switch
        {
            "ShipmentBooked" => "Booked",
            "CapacityReserved" => "CapacityReserved",
            "PaymentCaptured" => "PaymentCaptured",
            "LabelPrinted" => "LabelPrinted",
            "ParcelScanned" => document.scanType ?? Status,
            "DeliveryAttempted" => "DeliveryAttempted",
            "DeliveryDelayed" => "Delayed",
            "ParcelDelivered" => "Delivered",
            "RerouteRequested" => "RerouteRequested",
            "RerouteApproved" => "Rerouted",
            "PaymentRefunded" => "Refunded",
            _ => Status
        };

        return new Shipment(ShipmentId, document.version, status);
    }
}
