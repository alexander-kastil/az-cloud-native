namespace FoodApp.DataGenerator
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentAccount { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDate { get; set; }
    }
}