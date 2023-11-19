using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp
{
    public interface IPaymentRepository
    {        
        Task ExecutePayment(OrderEvent evt);
        Task AddPaymentAsync(PaymentTransaction Payment);
        Task<IEnumerable<PaymentTransaction>> GetAllPaymentsAsync();
        Task<PaymentTransaction> GetPaymentByIdAsync(string id, string customerId);
        Task UpdatePaymentAsync(string id, PaymentTransaction payment);
    }
}