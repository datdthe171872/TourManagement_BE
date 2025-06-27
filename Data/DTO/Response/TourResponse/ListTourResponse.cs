namespace TourManagement_BE.Data.DTO.Response.TourResponse
{
    public class ListTourResponse
    {
        public int TourId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string DurationInDays { get; set; } = null!;

        public string? StartPoint { get; set; }

        public string? Transportation { get; set; }

        public int TourOperatorId { get; set; }

        public int MaxSlots { get; set; }

        public int? SlotsBooked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string TourType { get; set; } = null!;

        public string? Note { get; set; }

        public string? TourStatus { get; set; }

        public double? AverageRating { get; set; }

        public bool IsActive { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyDescription { get; set; }
    }
}
