using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class SubmitJoinRequestDTO
    {
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
