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
    public class FeeRepository : GenericRepository<Fee>, IFeeRepository
    {
        public FeeRepository(ClubManagementContext context) : base(context)
        {
        }    
    }
}
