using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int UserId { get; set; }

    public int ClubId { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? JoinedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
