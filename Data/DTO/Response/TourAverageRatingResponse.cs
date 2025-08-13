using System;

namespace TourManagement_BE.Data.DTO.Response
{
    public class TourAverageRatingResponse
    {
        public int TourId { get; set; }
        public string TourTitle { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int Rating1Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating5Count { get; set; }
        public DateTime? LastRatingDate { get; set; }
    }
}
