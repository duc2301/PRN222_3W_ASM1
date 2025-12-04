using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Activity = ClubManagement.Repository.Models.Activity;

namespace ClubManagement.Repository.Repositories
{
    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(ClubManagementContext context) : base(context)
        {
        }

        public async Task<List<Activity>> GetAllActivitiesWithRelations()
        {
            return await _context.Set<Activity>()
                .Include(a => a.Club)
                .ToListAsync();
        }
        
        public async Task<Activity?> GetActivityWithRelationsById(int id)
        {
            return await _context.Set<Activity>()
                .Include(a => a.Club)
                .Include(a => a.ActivityParticipants)
                    .ThenInclude(ap => ap.User)
                .FirstOrDefaultAsync(a => a.ActivityId == id);
        }
    }
}
