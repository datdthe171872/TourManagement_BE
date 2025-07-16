namespace TourManagement_BE.Data.DTO.Request
{
    public class BookingSearchRequest
    {
        public int UserId { get; set; }
        public string? Keyword { get; set; }
    }
} 