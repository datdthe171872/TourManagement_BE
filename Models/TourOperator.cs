using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourOperator
{
    public int TourOperatorId { get; set; }

    public int? UserId { get; set; }

    public string? CompanyName { get; set; }

    public string? Description { get; set; }

    public string? CompanyLogo { get; set; }

    public string? LicenseNumber { get; set; }

    public DateOnly? LicenseIssuedDate { get; set; }

    public string? TaxCode { get; set; }

    public int? EstablishedYear { get; set; }

    public string? Hotline { get; set; }

    public string? Website { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Address { get; set; }

    public string? WorkingHours { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PurchaseTransaction> PurchaseTransactions { get; set; } = new List<PurchaseTransaction>();

    public virtual ICollection<PurchasedServicePackage> PurchasedServicePackages { get; set; } = new List<PurchasedServicePackage>();

    public virtual ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();

    public virtual ICollection<TourOperatorMedium> TourOperatorMedia { get; set; } = new List<TourOperatorMedium>();

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();

    public virtual User? User { get; set; }
}
