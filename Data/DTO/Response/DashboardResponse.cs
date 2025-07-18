using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class DashboardResponse
    {
        public int TotalBookings { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageValue { get; set; }
        public List<RecentBookingResponse> RecentBookings { get; set; } = new List<RecentBookingResponse>();
        public List<RecentInvoiceResponse> RecentInvoices { get; set; } = new List<RecentInvoiceResponse>();
    }

    public class RecentBookingResponse
    {
        public int BookingId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public DateTime SelectedDepartureDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? BookingStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public int? NumberOfAdults { get; set; }
        public int? NumberOfChildren { get; set; }
        public int? NumberOfInfants { get; set; }
    }

    public class RecentInvoiceResponse
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentReference { get; set; }
    }
} 