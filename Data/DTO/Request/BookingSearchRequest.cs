namespace TourManagement_BE.Data.DTO.Request
{
    public class BookingSearchCustomerRequest
    {
        public string? TourName { get; set; }
    }
    public class BookingSearchTourOperatorRequest
    {
        public string? TourName { get; set; }
        public string? UserName { get; set; }
    }
    public class BookingSearchAdminRequest
    {
        public string? TourName { get; set; }
        public string? UserName { get; set; }
    }
    // Giữ lại cho service nội bộ nếu cần
    public class BookingSearchRequest
    {
        public string? TourName { get; set; }
        public string? UserName { get; set; }
    }
} 