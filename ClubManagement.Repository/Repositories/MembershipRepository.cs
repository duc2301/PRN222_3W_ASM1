using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        public MembershipRepository(ClubManagementContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Membership> Items, int TotalCount)> GetByClubAsync(
          int clubId, string? search, string? roleFilter,
            string? statusFilter, int page,  int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Memberships
                .Include(m => m.User)
                .Where(m => m.ClubId == clubId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(m =>
                    m.User.FullName.ToLower().Contains(search) ||
                    m.User.Email.ToLower().Contains(search) ||
                    m.User.Username.ToLower().Contains(search));
            }

            // Filter theo Role
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                query = query.Where(m => m.Role == roleFilter);
            }

            // Filter theo Status
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(m => m.Status == statusFilter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Membership?> GetByUserAndClubAsync(int userId, int clubId)
        {
            return await _context.Memberships
                                 .FirstOrDefaultAsync(m => m.UserId == userId
                                                        && m.ClubId == clubId);
        }

        public async Task<List<Membership>> GetActiveMembersByClubIdAsync(int clubId)
        {
            return await _context.Memberships
                .Where(m => m.ClubId == clubId && m.Status == "Active")
                .ToListAsync();
        }
    }
}
