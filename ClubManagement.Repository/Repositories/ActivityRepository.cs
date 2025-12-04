using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Repositories.Interfaces;
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
    }
}
