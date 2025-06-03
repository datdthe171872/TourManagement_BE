using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class PaymentType
{
    public Guid PaymentTypeId { get; set; }

    public string PaymentTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
