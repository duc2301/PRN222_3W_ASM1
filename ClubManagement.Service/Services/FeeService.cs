using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ClubManagement.Service.Services
{
    public class FeeService
    {
        private readonly ClubManagementContext _context;

        public FeeService(ClubManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Fee>> GetAllAsync()
            => await _context.Fees.ToListAsync();

        public async Task<Fee?> GetByIdAsync(int id)
            => await _context.Fees.FindAsync(id);

        public async Task CreateAsync(Fee fee)
        {
            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fee fee)
        {
            _context.Fees.Update(fee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var fee = await _context.Fees.FindAsync(id);
            if (fee != null)
            {
                _context.Fees.Remove(fee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
