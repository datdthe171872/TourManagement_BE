using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class ServicePackage
{
    public int PackageId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationInDay { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PurchaseTransaction> PurchaseTransactions { get; set; } = new List<PurchaseTransaction>();

    public virtual ICollection<PurchasedServicePackage> PurchasedServicePackages { get; set; } = new List<PurchasedServicePackage>();

    public virtual ICollection<ServicePackageFeature> ServicePackageFeatures { get; set; } = new List<ServicePackageFeature>();
}
