using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Contract
{
    public Guid ContractId { get; set; }

    public Guid BookingId { get; set; }

    public Guid TourOperatorId { get; set; }

    public string ContractFileUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public bool? IsSignedByCustomer { get; set; }

    public DateTime? SignedAt { get; set; }

    public string? DigitalSignature { get; set; }

    public bool? CreatedByTourOperator { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual TourOperator TourOperator { get; set; } = null!;
}
