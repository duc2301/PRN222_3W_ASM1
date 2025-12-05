using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {

        Task<List<Payment>> GetByUserAsync(int userId);
        Task<List<Payment>> GetByFeeAsync(int feeId);

        Task<bool> HasPaidAsync(int userId, int feeId);
        Task<List<Payment>> GetAllWithDetailsAsync();
        Task<Payment?> GetByIdAsync(int id);

    }
}
