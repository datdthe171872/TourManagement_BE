using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class BookingExtraCharge
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int ExtraChargeId { get; set; }

    public string? Content { get; set; }

    public int? Quantity { get; set; }

    public bool IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ExtraCharge ExtraCharge { get; set; } = null!;
}
