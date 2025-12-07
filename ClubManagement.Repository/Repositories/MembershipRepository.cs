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

        public async Task<List<Membership>> GetByClubIdAsync(int clubId)
        {
            return await _context.Memberships
                .Where(m => m.ClubId == clubId)
                .ToListAsync();
        }

        public async Task<List<Membership>> GetActiveMembersByClubIdAsync(int clubId)
        {
            return await _context.Memberships
                .Where(m => m.ClubId == clubId && m.Status == "Active")
                .Include(m => m.User)
                .ToListAsync();
        }
    }
}
