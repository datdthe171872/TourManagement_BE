namespace TourManagement_BE.Data.DTO.Response.TourBookingResponse
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentReference { get; set; }
    }

}
