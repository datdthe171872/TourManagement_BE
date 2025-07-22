using TourManagement_BE.Data.Models;
using TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse;

namespace TourManagement_BE.Data.DTO.Response.TourBookingDetailResponse
{
    public class BookingExtraChargesDTO
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public int ExtraChargeId { get; set; }

        public string? Content { get; set; }

        public int? Quantity { get; set; }

        public bool IsActive { get; set; }

        public  List<ExtraChargeDto> ExtraCharge { get; set; } = null!;
    }
}
