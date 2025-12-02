using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class ClubResponseDTO
    {
        public int ClubId { get; set; }

        public string ClubName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? LeaderId { get; set; }

        public virtual User? Leader { get; set; }

    }
}
