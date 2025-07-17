namespace TourManagement_BE.Data.DTO.Request.Payment
{
    public class VnpayPaymentCallbackRequest
    {
        public int BookingId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentReference { get; set; } 
        public string PaymentMethod { get; set; } = "VNPAY";
    }

}
