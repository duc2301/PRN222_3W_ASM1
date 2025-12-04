namespace ClubManagement.Service.DTOs.ResponseDTOs;

public class ActivityParticipantResponseDTO
{
    public int ParticipantId { get; set; }

    public int ActivityId { get; set; }

    public int UserId { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? Status { get; set; }
    
    public UserResponseDTO User { get; set; } = null!;
}