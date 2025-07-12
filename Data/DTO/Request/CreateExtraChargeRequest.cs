namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateExtraChargeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
    }
} 