using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class LoginDTO
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; }
    }
}
