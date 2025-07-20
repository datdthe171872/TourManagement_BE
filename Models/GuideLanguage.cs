using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class GuideLanguage
{
    public int Id { get; set; }

    public int? GuideId { get; set; }

    public int? LanguageId { get; set; }

    public bool IsActive { get; set; }

    public virtual TourGuide? Guide { get; set; }

    public virtual Language? Language { get; set; }
}
