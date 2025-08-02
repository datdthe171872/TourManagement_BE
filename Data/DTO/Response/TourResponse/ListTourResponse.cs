namespace TourManagement_BE.Data.DTO.Response.TourResponse
{
    public class ListTourResponse
    {
        public int TourId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PriceOfAdults { get; set; }

        public decimal PriceOfChildren { get; set; }

        public decimal PriceOfInfants { get; set; }

        public string DurationInDays { get; set; } = null!;

        public string? StartPoint { get; set; }

        public string? Transportation { get; set; }

        public int TourOperatorId { get; set; }

        public int MaxSlots { get; set; }

        public int MinSlots { get; set; }

        public int? SlotsBooked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Note { get; set; }

        public string? TourStatus { get; set; }

        public string TourAvartar { get; set; } = null!;

        public double? AverageRating { get; set; }

        public bool IsActive { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyDescription { get; set; }
    }
}
