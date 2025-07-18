using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourGuide
{
    public int TourGuideId { get; set; }

    public int? UserId { get; set; }

    public int? TourOperatorId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();

    public virtual ICollection<TourAcceptanceReport> TourAcceptanceReports { get; set; } = new List<TourAcceptanceReport>();

    public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>();

    public virtual TourOperator? TourOperator { get; set; }

    public virtual User? User { get; set; }
}
