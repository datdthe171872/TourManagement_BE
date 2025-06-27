namespace TourManagement_BE.Data.DTO.Response.PaymentResponse
{
    public class ViewHistoryPaymentPackageResponse
    {
        public int TransactionId { get; set; }

        public int TourOperatorId { get; set; }

        public string TourOperatorName { get; set; } = null!;
        
        public int PackageId { get; set; }
        
        public string PackageName { get; set; } = null!;

        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
