using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class UpdateMemberRequestDTO
    {
        public int ClubId { get; set; }

        public int UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
