using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Language
{
    public Guid LanguageId { get; set; }

    public string LanguageName { get; set; } = null!;

    public virtual ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();
}
