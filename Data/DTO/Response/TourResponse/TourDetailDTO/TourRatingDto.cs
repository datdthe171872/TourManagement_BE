using TourManagement_BE.Models;

namespace TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO
{
    public class TourRatingDto
    {
        public int RatingId { get; set; }

        public int UserId { get; set; }

        public string? TourRating_Username { get; set; }

        public int? Rating { get; set; }

        public string? Comment { get; set; }

        public string? MediaUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsActive { get; set; }

    }
}
