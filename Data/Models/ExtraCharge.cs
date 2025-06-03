using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class ExtraCharge
{
    public Guid ExtraChargeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BookingExtraCharge> BookingExtraCharges { get; set; } = new List<BookingExtraCharge>();
}
