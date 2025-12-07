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

        public async Task<Payment> CreatePaymentAsync(int userId, int feeId, decimal amount)
        {
            var payment = new Payment
            {
                UserId = userId,
                FeeId = feeId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = "Pending" // Đổi thành Pending để ClubManager xác nhận
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
            return await _paymentRepo.HasPaidAsync(userId, feeId);
        }
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

        // Student xác nhận đóng phí (chuyển từ Unpaid sang Paid - tự động xác nhận)
        public async Task RequestPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments
                .AsTracking()
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new Exception("Payment not found");

            if (payment.Status == "Paid")
                throw new Exception("Payment already confirmed");

            if (payment.Status == "Expired")
                throw new Exception("Payment has expired");

            // Chuyển từ Unpaid sang Paid (tự động xác nhận khi student đóng)
            payment.Status = "Paid";
            payment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // Kiểm tra và cập nhật trạng thái quá hạn
        public async Task CheckAndUpdateExpiredPaymentsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var payments = await _context.Payments
                .Include(p => p.Fee)
                .Where(p => (p.Status == "Unpaid" || p.Status == "Pending") && 
                           p.Fee.DueDate < today)
                .ToListAsync();

            foreach (var payment in payments)
            {
                payment.Status = "Expired";
            }

            if (payments.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

    }
}
