using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourMedium
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public string? MediaUrl { get; set; }

    public string? MediaType { get; set; }

    public bool IsActive { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
