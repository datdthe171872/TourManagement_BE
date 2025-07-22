using TourManagement_BE.Data.DTO.Response.TourBookingDetailResponse;
using TourManagement_BE.Mapping.TourBookingDetail;
using TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse;

namespace TourManagement_BE.Data.DTO.Response.TourBookingResponse
{
    public class TourBookingDetailResponse
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public int TourId { get; set; }

        public int DepartureDateId { get; set; }

        public DateTime? DepartureDate { get; set; }

        public DateTime? BookingDate { get; set; }

        public int? NumberOfAdults { get; set; }

        public int? NumberOfChildren { get; set; }

        public int? NumberOfInfants { get; set; }

        public string? NoteForTour { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? Contract { get; set; }

        public string? BookingStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public bool IsActive { get; set; }

        public List<PaymentDto> Payments { get; set; } = new();
        public TourAcceptanceReportDto? AcceptanceReport { get; set; }
        public List<BookingExtraChargesDTO>? BookingExtraCharges { get; set; } = new();
    }

}
