using System;
using System.Collections.Generic;

namespace ClubManagement.Repository.Models;

public partial class ActivityParticipant
{
    public int ParticipantId { get; set; }

    public int ActivityId { get; set; }

    public int UserId { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? Status { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
