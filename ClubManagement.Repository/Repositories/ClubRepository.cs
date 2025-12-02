using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        public ClubRepository(ClubManagementContext context) : base(context)
        {
        }
    }
}
