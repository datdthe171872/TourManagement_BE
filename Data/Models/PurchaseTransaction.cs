using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class PurchaseTransaction
{
    public int TransactionId { get; set; }

    public int TourOperatorId { get; set; }

    public int PackageId { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? ContentCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ServicePackage Package { get; set; } = null!;

    public virtual ICollection<PurchasedServicePackage> PurchasedServicePackages { get; set; } = new List<PurchasedServicePackage>();

    public virtual TourOperator TourOperator { get; set; } = null!;
}
