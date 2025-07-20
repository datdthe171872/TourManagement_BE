using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourOperatorMedia
{
    public int Id { get; set; }

    public int TourOperatorId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public DateTime? UploadedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual TourOperator TourOperator { get; set; } = null!;
}
