using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class DepartureDate
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public DateTime DepartureDate1 { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Tour Tour { get; set; } = null!;
}
