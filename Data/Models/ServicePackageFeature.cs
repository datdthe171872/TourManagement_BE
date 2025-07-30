using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class ServicePackageFeature
{
    public int FeatureId { get; set; }

    public int PackageId { get; set; }

    public int NumberOfTours { get; set; }

    public int NumberOfTourAttribute { get; set; }

    public bool PostVideo { get; set; }

    public bool IsActive { get; set; }

    public virtual ServicePackage Package { get; set; } = null!;
}
