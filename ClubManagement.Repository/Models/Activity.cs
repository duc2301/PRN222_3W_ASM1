using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public int ClubId { get; set; }

    public string ActivityName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<ActivityParticipant> ActivityParticipants { get; set; } = new List<ActivityParticipant>();

    public virtual Club Club { get; set; } = null!;
}
