using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class PurchasedServicePackage
{
    public int PurchaseId { get; set; }

    public int TourOperatorId { get; set; }

    public int PackageId { get; set; }

    public int TransactionId { get; set; }

    public DateTime ActivationDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? NumOfToursUsed { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ServicePackage Package { get; set; } = null!;

    public virtual TourOperator TourOperator { get; set; } = null!;

    public virtual PurchaseTransaction Transaction { get; set; } = null!;
}
