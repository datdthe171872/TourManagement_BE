using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string LanguageName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();
}
