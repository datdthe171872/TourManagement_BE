using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class ServicePackageFeature
{
    public int FeatureId { get; set; }

    public int PackageId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string FeatureValue { get; set; } = null!;

    public virtual ServicePackage Package { get; set; } = null!;
}
