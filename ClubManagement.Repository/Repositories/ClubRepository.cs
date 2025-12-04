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
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        private readonly ClubManagementContext _context;

        public ClubRepository(ClubManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Clubs.AnyAsync(c => c.ClubName == name);
        }
    }
}
