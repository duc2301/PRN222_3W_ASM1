using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;

namespace ClubManagement.Repository.Repositories
{
    public class ActivityParticipantRepository : GenericRepository<ActivityParticipant>, IActivityParticipantRepository
    {
        public ActivityParticipantRepository(ClubManagementContext context) : base(context)
        {
        }
    }
}
