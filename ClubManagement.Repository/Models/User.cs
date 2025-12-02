using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Department { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ActivityParticipant> ActivityParticipants { get; set; } = new List<ActivityParticipant>();

    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();

    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
