using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourImage
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
