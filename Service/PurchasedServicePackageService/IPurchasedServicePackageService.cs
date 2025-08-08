using TourManagement_BE.Data.DTO.Request.PurchaseServicePackage;

namespace TourManagement_BE.Service.PurchasedServicePackageService
{
    public interface IPurchasedServicePackageService
    {
        Task<PurchaseResult> PurchaseServicePackageAsync(PurchaseServicePackagesRequest request);
        Task<PaymentResult> ProcessPaymentWebhookAsync(PaymentNotification payload);
    }

    public class PurchaseResult
    {
        public string Message { get; set; }
        public int TransactionId { get; set; }
        public string ContentCode { get; set; }
    }

    public class PaymentResult
    {
        public string Message { get; set; }
    }
}
