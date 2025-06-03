using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public decimal? AmountPaid { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public Guid PaymentTypeId { get; set; }

    public string? PaymentReference { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
