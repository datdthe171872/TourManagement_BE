namespace TourManagement_BE.Data.DTO.Response.PaymentResponse
{
    public class ViewAllPaymentHistory
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? RoleName { get; set; }

        public decimal Amount { get; set; }

        public decimal? AmountPaid { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int PaymentTypeId { get; set; }

        public string? PaymentTypeName { get; set; }

        public string? PaymentReference { get; set; }

        public bool IsActive { get; set; }
    }
}
