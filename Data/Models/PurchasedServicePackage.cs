using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class PurchasedServicePackage
{
    public Guid PurchaseId { get; set; }

    public Guid TourOperatorId { get; set; }

    public Guid PackageId { get; set; }

    public Guid TransactionId { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? NumOfToursUsed { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ServicePackage Package { get; set; } = null!;

    public virtual TourOperator TourOperator { get; set; } = null!;

    public virtual PurchaseTransaction Transaction { get; set; } = null!;
}
