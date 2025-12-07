using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IMembershipRepository :IGenericRepository<Membership>
    {
        Task<(IEnumerable<Membership> Items, int TotalCount)> GetByClubAsync(
          int clubId,
          string? search,
            string? roleFilter,
            string? statusFilter,
          int page,
          int pageSize);

        Task<Membership?> GetByUserAndClubAsync(int userId, int clubId);
    }
}
