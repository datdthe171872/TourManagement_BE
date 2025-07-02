using System.Collections.Generic;

namespace Data.DTO.Response
{
    public class TourOperatorDashboardResponse
    {
        public int TotalTours { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
        public List<MonthlyStat> MonthlyBookingStats { get; set; }
        public List<MonthlyStat> MonthlyRevenueStats { get; set; }
        public List<TopTourStat> TopTours { get; set; }
    }

    public class MonthlyStat
    {
        public string Month { get; set; } // "yyyy-MM"
        public int Value { get; set; }
    }

    public class TopTourStat
    {
        public int TourId { get; set; }
        public string Name { get; set; }
        public int Bookings { get; set; }
    }
} 