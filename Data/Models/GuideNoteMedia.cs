using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class GuideNoteMedia

{
    public int Id { get; set; }

    public int NoteId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual GuideNote Note { get; set; } = null!;
}
