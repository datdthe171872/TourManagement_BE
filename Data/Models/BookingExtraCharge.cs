using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class BookingExtraCharge
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid ExtraChargeId { get; set; }

    public int? Quantity { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ExtraCharge ExtraCharge { get; set; } = null!;
}
