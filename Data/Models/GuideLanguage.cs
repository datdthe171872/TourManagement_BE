using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class GuideLanguage
{
    public Guid Id { get; set; }

    public Guid? GuideId { get; set; }

    public Guid? LanguageId { get; set; }

    public virtual TourGuide? Guide { get; set; }

    public virtual Language? Language { get; set; }
}
