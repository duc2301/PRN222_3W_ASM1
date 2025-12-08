using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class ApproveJoinRequestDTO
    {
        public int RequestId { get; set; }
        public int LeaderId { get; set; }
        public string? Reason { get; set; }
    }
}
