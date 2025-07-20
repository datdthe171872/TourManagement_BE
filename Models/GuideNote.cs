using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class GuideNote
{
    public int NoteId { get; set; }

    public int AssignmentId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual TourGuideAssignment Assignment { get; set; } = null!;

    public virtual ICollection<GuideNoteMedium> GuideNoteMedia { get; set; } = new List<GuideNoteMedium>();
}
