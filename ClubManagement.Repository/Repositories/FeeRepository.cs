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
    public class FeeRepository : GenericRepository<Fee>, IFeeRepository
    {
        public FeeRepository(ClubManagementContext context) : base(context) { }

        public async Task<List<Fee>> GetByClubAsync(int clubId)
        {
            return await _context.Fees
                .Include(f => f.Club)
                .Where(f => f.ClubId == clubId)
                .ToListAsync();
        }

        public async Task<List<Fee>> GetAllWithClubAsync()
        {
            return await _context.Fees
                .Include(f => f.Club)
                .ToListAsync();
        }

        public async Task<Fee> GetByIdWithClubAsync(int id)
        {
            return await _context.Fees
                .Include(f => f.Club)
                .FirstOrDefaultAsync(f => f.FeeId == id);
        }
    }
}
