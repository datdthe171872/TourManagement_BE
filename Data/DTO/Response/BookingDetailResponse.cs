using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class BookingDetailResponse
    {
        public int BookingId { get; set; }
        
        // Tour Information
        public TourInfo Tour { get; set; } = new TourInfo();
        
        // Booking Information
        public BookingInfo Booking { get; set; } = new BookingInfo();
        
        // Guest Information
        public GuestInfo Guest { get; set; } = new GuestInfo();
        
        // Billing Information
        public BillingInfo BillingInfo { get; set; } = new BillingInfo();
        
        // Payment Information
        public PaymentInfo PaymentInfo { get; set; } = new PaymentInfo();
        
        // Guide Notes Information
        public List<GuideNotesInfo> GuideNotes { get; set; } = new List<GuideNotesInfo>();
    }

    public class TourInfo
    {
        public string Title { get; set; } = string.Empty;
        public int MaxSlots { get; set; }
        public string? Transportation { get; set; }
        public string? StartPoint { get; set; }
        public DateTime? DepartureDate { get; set; }
    }

    public class BookingInfo
    {
        public DateTime? BookingDate { get; set; }
        public string? Contract { get; set; }
        public string? NoteForTour { get; set; }
    }

    public class GuestInfo
    {
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public int TotalGuests => NumberOfAdults + NumberOfChildren + NumberOfInfants;
    }

    public class BillingInfo
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class PaymentInfo
    {
        public decimal? TotalPrice { get; set; }
        public string? PaymentStatus { get; set; }
        public string? BookingStatus { get; set; }
    }
    
    public class GuideNotesInfo
    {
        public int NoteId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public decimal? ExtraCost { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 