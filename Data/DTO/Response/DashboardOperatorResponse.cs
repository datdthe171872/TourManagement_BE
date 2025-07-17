using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class DashboardOperatorResponse
    {
        public int TotalTours { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalFeedbacks { get; set; }
        public List<BookingStatisticsResponse> BookingStatistics { get; set; } = new List<BookingStatisticsResponse>();
        public decimal TotalEarningsThisYear { get; set; }
        public List<MonthlyEarningResponse> MonthlyEarnings { get; set; } = new List<MonthlyEarningResponse>();
        public List<RecentlyAddedTourResponse> RecentlyAddedTours { get; set; } = new List<RecentlyAddedTourResponse>();
        public List<LatestInvoiceResponse> LatestInvoices { get; set; } = new List<LatestInvoiceResponse>();
    }

    public class BookingStatisticsResponse
    {
        public string TourType { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyEarningResponse
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Earning { get; set; }
    }

    public class RecentlyAddedTourResponse
    {
        public int TourId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string TourType { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public int BookingCount { get; set; }
    }

    public class LatestInvoiceResponse
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
} 