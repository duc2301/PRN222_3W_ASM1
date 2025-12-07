using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class ClubMemberListItemDTO
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;


        public string? Department { get; set; }

        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? JoinedAt { get; set; }
    }

    // ViewModel cho tab Thành viên (bao gồm club + list + phân trang)
    public class ClubMembersPageViewModel
    {
        public ClubResponseDTO Club { get; set; } = null!;
        public List<ClubMemberListItemDTO> Members { get; set; } = new();

        public string? Search { get; set; }
        public string? RoleFilter { get; set; }
        public string? StatusFilter { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
