using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class TourComparisonResponse
    {
        public TourComparisonInfo Tour1 { get; set; } = new();
        public TourComparisonInfo Tour2 { get; set; } = new();
        public ComparisonResult Result { get; set; } = new();
    }

    public class TourComparisonInfo
    {
        public int TourId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DurationInDays { get; set; } = string.Empty;
        public decimal PriceOfAdults { get; set; }
        public decimal PriceOfChildren { get; set; }
        public decimal PriceOfInfants { get; set; }
        public int MaxSlots { get; set; }
        public int SlotsBooked { get; set; }
        public double OccupancyRate { get; set; } // Tỷ lệ đặt chỗ
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public string StartPoint { get; set; } = string.Empty;
        public string Transportation { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }

    public class ComparisonResult
    {
        public int Tour1Wins { get; set; }
        public int Tour2Wins { get; set; }
        public int Ties { get; set; }
        public string Winner { get; set; } = string.Empty;
        public List<CriterionComparison> CriterionComparisons { get; set; } = new();
    }

    public class CriterionComparison
    {
        public string CriterionName { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty; // "Tour1", "Tour2", "Tie"
        public string Description { get; set; } = string.Empty;
        public double Tour1Value { get; set; }
        public double Tour2Value { get; set; }
    }
}
