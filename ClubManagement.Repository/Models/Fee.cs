using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class Fee
{
    public int FeeId { get; set; }

    public int ClubId { get; set; }

    public string Title { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly DueDate { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
