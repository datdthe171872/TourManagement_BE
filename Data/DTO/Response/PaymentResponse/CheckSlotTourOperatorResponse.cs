namespace TourManagement_BE.Data.DTO.Response.PaymentResponse
{
    public class CheckSlotTourOperatorResponse
    {
        public int PurchaseId { get; set; }

        public int TourOperatorId { get; set; }

        public string? TourOperatorName { get; set; }

        public int PackageId { get; set; }
         
        public string? PackageName { get; set; }

        public int TransactionId { get; set; }

        public DateTime ActivationDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
