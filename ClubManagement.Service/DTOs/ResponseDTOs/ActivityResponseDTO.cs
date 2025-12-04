using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class ActivityResponseDTO
    {
        public int ActivityId { get; set; }

        public int ClubId { get; set; }

        public string ActivityName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Location { get; set; }

        public virtual Club? Club { get; set; }
    }
}
