using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class JoinRequestRequestDTO
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? UserFullName { get; set; }
        public string? ClubName { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}
