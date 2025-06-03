using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public Guid TourId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int? NumberOfAdults { get; set; }

    public int? NumberOfChildren { get; set; }

    public int? NumberOfInfants { get; set; }

    public string? NoteForTour { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? DepositAmount { get; set; }

    public decimal? RemainingAmount { get; set; }

    public string? BookingStatus { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual ICollection<BookingExtraCharge> BookingExtraCharges { get; set; } = new List<BookingExtraCharge>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Tour Tour { get; set; } = null!;

    public virtual ICollection<TourCompletionReport> TourCompletionReports { get; set; } = new List<TourCompletionReport>();

    public virtual User User { get; set; } = null!;
}
