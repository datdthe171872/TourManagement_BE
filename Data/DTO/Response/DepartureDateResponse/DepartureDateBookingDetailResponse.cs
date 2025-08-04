using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse
{
    public class DepartureDateBookingDetailResponse
    {
        public int BookingId { get; set; }
        public TourDetailInfo Tour { get; set; } = new TourDetailInfo();
        public BookingDetailInfo Booking { get; set; } = new BookingDetailInfo();
        public GuestDetailInfo Guest { get; set; } = new GuestDetailInfo();
        public BillingDetailInfo BillingInfo { get; set; } = new BillingDetailInfo();
        public PaymentDetailInfo PaymentInfo { get; set; } = new PaymentDetailInfo();
        public List<GuideNoteDetailInfo> GuideNotes { get; set; } = new List<GuideNoteDetailInfo>();
    }

    public class TourDetailInfo
    {
        public string Title { get; set; } = string.Empty;
        public int? MaxSlots { get; set; }
        public string? Transportation { get; set; }
        public string? StartPoint { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string? DurationInDays { get; set; }
    }

    public class BookingDetailInfo
    {
        public DateTime BookingDate { get; set; }
        public string? Contract { get; set; }
        public string? NoteForTour { get; set; }
    }

    public class GuestDetailInfo
    {
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public int TotalGuests => NumberOfAdults + NumberOfChildren + NumberOfInfants;
    }

    public class BillingDetailInfo
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class PaymentDetailInfo
    {
        public decimal? TotalPrice { get; set; }
        public string? PaymentStatus { get; set; }
        public string? BookingStatus { get; set; }
    }

    public class GuideNoteDetailInfo
    {
        public int NoteId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}