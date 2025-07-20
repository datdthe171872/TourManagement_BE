using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int TourId { get; set; }

    public int DepartureDateId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int? NumberOfAdults { get; set; }

    public int? NumberOfChildren { get; set; }

    public int? NumberOfInfants { get; set; }

    public string? NoteForTour { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Contract { get; set; }

    public string? BookingStatus { get; set; }

    public string? PaymentStatus { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<BookingExtraCharge> BookingExtraCharges { get; set; } = new List<BookingExtraCharge>();

    public virtual DepartureDate DepartureDate { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Tour Tour { get; set; } = null!;

    public virtual ICollection<TourAcceptanceReport> TourAcceptanceReports { get; set; } = new List<TourAcceptanceReport>();

    public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>();

    public virtual User User { get; set; } = null!;
}
