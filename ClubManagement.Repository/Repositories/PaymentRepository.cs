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
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ClubManagementContext context) : base(context) { }

        public async Task<List<Payment>> GetByUserAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByFeeAsync(int feeId)
        {
            return await _context.Payments
                .Where(p => p.FeeId == feeId)
                .ToListAsync();
        }
        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Fee)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }


        public async Task<bool> HasPaidAsync(int userId, int feeId)
        {
            return await _context.Payments
                .AnyAsync(p => p.UserId == userId && p.FeeId == feeId && p.Status == "Paid");
        }
        public async Task<List<Payment>> GetAllWithDetailsAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Fee)
                .ToListAsync();
        }
    }
}
