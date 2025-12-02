using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class JoinRequest
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    public int ClubId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
