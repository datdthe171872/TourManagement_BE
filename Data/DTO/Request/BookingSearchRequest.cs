namespace TourManagement_BE.Data.DTO.Request
{
    public class BookingSearchCustomerRequest
    {
        public string? Keyword { get; set; }
    }
    public class BookingSearchTourOperatorRequest
    {
        public string? Keyword { get; set; }
    }
    public class BookingSearchAdminRequest
    {
        public string? Keyword { get; set; }
    }
    // Giữ lại cho service nội bộ nếu cần
    public class BookingSearchRequest
    {
        public string? Keyword { get; set; }
    }
} 