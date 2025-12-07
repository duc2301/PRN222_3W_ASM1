using System;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class FeeResponseDTO
    {
        public int FeeId { get; set; }
        public int ClubId { get; set; }
        public string ClubName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

