using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public string ClubName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? LeaderId { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    public virtual User? Leader { get; set; }

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
