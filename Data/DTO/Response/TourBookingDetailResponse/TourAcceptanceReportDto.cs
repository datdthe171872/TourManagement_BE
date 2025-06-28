namespace TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse
{
    public class TourAcceptanceReportDto
    {
        public int ReportId { get; set; }
        public int TourGuideId { get; set; }
        public DateTime? ReportDate { get; set; }
        public string? Summary { get; set; }
        public decimal? TotalExtraCost { get; set; }
        public string? Notes { get; set; }
        public string? AttachmentUrl { get; set; }
    }

}
