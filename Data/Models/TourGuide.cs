using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourGuide
{
    public Guid TourGuideId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? TourOperatorId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();

    public virtual ICollection<GuideRating> GuideRatings { get; set; } = new List<GuideRating>();

    public virtual ICollection<TourCompletionReport> TourCompletionReports { get; set; } = new List<TourCompletionReport>();

    public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>();

    public virtual TourOperator? TourOperator { get; set; }

    public virtual User? User { get; set; }
}
