using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourExperience
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public string? Content { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
