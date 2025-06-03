using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourOperator
{
    public Guid TourOperatorId { get; set; }

    public Guid? UserId { get; set; }

    public string? CompanyName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<PurchaseTransaction> PurchaseTransactions { get; set; } = new List<PurchaseTransaction>();

    public virtual ICollection<PurchasedServicePackage> PurchasedServicePackages { get; set; } = new List<PurchasedServicePackage>();

    public virtual ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();

    public virtual User? User { get; set; }
}
