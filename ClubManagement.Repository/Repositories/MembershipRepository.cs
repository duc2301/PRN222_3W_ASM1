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
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        public MembershipRepository(ClubManagementContext context) : base(context)
        {
        }    
    }
}
