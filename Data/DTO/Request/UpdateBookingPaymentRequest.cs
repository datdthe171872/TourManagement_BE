using Microsoft.AspNetCore.Http;

namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateBookingPaymentRequest
    {
        public int BookingId { get; set; }
        public IFormFile PaymentImage { get; set; }
    }
}