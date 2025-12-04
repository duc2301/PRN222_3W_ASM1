using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClubManagement.Repository.Repositories
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        public ClubRepository(ClubManagementContext context) : base(context)
        {
        }

        public async Task<List<Club>> GetAllAsync()
        {
            return await _context.Clubs
                .Include(c => c.Leader)
                .ToListAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            return await _context.Clubs
                .Include(c => c.Leader)
                .FirstOrDefaultAsync(c => c.ClubId == id);
        }
    }
}
