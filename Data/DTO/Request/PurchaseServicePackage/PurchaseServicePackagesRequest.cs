namespace TourManagement_BE.Data.DTO.Request.PurchaseServicePackage
{
    public class PurchaseServicePackagesRequest
    {
        public int TourOperatorId { get; set; }

        public int PackageId { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public string ContentCode { get; set; }
    }
}
