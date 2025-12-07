using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ClubManagementContext _context;

        public PaymentService(
           IPaymentRepository paymentRepo,
           ClubManagementContext context)
        {
            _paymentRepo = paymentRepo;
            _context = context;
        }

        // Tạo payment dưới dạng "Pending" (chờ duyệt)
        public async Task<Payment> CreatePaymentAsync(int userId, int feeId, decimal amount)
        {
            var payment = new Payment
            {
                UserId = userId,
                FeeId = feeId,
                Amount = amount,
                // Chưa duyệt => PaymentDate null
                PaymentDate = null,
                Status = "Pending"
            };

            await _paymentRepo.CreateAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _paymentRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _paymentRepo.GetAllWithDetailsAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserAsync(int userId)
        {
            return await _paymentRepo.GetByUserAsync(userId);
        }

        public async Task<bool> HasPaidAsync(int userId, int feeId)
        {
            // Kiểm tra đã có bản ghi "Paid" cho user + fee hay chưa
            return await _paymentRepo.HasPaidAsync(userId, feeId);
        }

        // Dùng để duyệt: set Paid và set PaymentDate
        public async Task MarkAsPaidAsync(int paymentId)
        {
            var payment = await _context.Payments
                .AsTracking()
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new Exception("Payment not found");

            payment.Status = "Paid";
            payment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

    }
}
