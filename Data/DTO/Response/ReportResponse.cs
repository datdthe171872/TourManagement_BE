namespace TourManagement_BE.Data.DTO.Response
{
    public class ReportResponse
    {
        public string Username { get; set; }
        public int BookingId { get; set; }
        public string TourTitle { get; set; }
        public string Contract { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalExtraCost { get; set; }
        public decimal Total { get; set; }
    }
} 