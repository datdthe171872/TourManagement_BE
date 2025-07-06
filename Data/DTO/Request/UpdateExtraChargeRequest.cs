namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateExtraChargeRequest
    {
        public int ExtraChargeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsActive { get; set; }
    }
} 