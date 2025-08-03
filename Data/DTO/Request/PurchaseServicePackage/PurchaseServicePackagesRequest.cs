namespace TourManagement_BE.Data.DTO.Request.PurchaseServicePackage
{
    public class PurchaseServicePackagesRequest
    {
        public int UserId { get; set; }

        public int PackageId { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        public int NumberYearActive { get; set; }
    }
}
