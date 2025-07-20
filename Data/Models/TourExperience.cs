using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourExperience
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public string? Content { get; set; }

    public bool IsActive { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
