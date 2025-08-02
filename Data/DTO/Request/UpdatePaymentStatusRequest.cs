namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdatePaymentStatusRequest
    {
        public int BookingId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
} 