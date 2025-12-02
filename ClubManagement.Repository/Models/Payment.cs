using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int FeeId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Fee Fee { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
