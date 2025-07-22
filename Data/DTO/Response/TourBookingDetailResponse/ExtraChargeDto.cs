namespace TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse
{
    public class ExtraChargeDto
    {
        public int ExtraChargeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
    }

}
