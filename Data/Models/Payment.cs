using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public decimal? AmountPaid { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int PaymentTypeId { get; set; }

    public string? PaymentReference { get; set; }

    public bool IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
