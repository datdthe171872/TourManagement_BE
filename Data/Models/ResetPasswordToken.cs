using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class ResetPasswordToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public virtual User User { get; set; } = null!;
}
