namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateBookingStatusRequest
    {
        public int BookingId { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
    }
} 