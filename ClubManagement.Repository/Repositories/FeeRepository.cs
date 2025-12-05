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
                .Where(f => f.ClubId == clubId)
                .ToListAsync();
        }
    }
}
