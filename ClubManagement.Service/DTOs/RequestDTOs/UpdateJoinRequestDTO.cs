using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class UpdateJoinRequestDTO
    {
        public string Status { get; set; } = null!; // Approved/Rejected
        public string? Note { get; set; }
    }
}
