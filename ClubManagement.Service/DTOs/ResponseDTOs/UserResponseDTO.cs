using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Department { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
