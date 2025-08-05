using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse;

public class DepartureDateWithBookingResponse
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourTitle { get; set; } = null!;
    public DateTime DepartureDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsCancelDate { get; set; }
    public int TotalBookings { get; set; }
    public int AvailableSlots { get; set; }
    public List<BookingInfo> Bookings { get; set; } = new List<BookingInfo>();
    public List<TourGuideInfo> TourGuides { get; set; } = new List<TourGuideInfo>();
}

public class BookingInfo
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateTime? BookingDate { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfChildren { get; set; }
    public int? NumberOfInfants { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? BookingStatus { get; set; }
    public string? PaymentStatus { get; set; }
} 