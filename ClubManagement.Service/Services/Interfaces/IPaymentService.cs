using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int userId, int feeId, decimal amount);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByUserAsync(int userId);
        Task<bool> HasPaidAsync(int userId, int feeId);
        Task MarkAsPaidAsync(int paymentId);


        Task<Payment> GetByIdAsync(int id);
        Task RequestPaymentAsync(int paymentId);
        Task CheckAndUpdateExpiredPaymentsAsync();

    }
}
