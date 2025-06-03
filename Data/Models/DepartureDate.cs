using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class DepartureDate
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public DateOnly DepartureDate1 { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
