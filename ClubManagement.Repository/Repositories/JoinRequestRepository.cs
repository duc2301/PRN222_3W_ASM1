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
    public class JoinRequestRepository
    : GenericRepository<JoinRequest>, IJoinRequestRepository
    {
        public JoinRequestRepository(ClubManagementContext context) : base(context) { }

        public async Task<List<JoinRequest>> GetByUserAsync(int userId)
        {
            return await _context.JoinRequests
                .Where(j => j.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<JoinRequest>> GetByClubAsync(int clubId)
        {
            return await _context.JoinRequests
                .Where(j => j.ClubId == clubId)
                .ToListAsync();
        }
        public async Task<JoinRequest?> GetByIdAsync(int requestId)
        {
            return await _context.JoinRequests
                .FirstOrDefaultAsync(j => j.RequestId == requestId); // sửa đúng tên key!
        }
    }
}
