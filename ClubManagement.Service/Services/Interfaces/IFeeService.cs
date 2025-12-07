using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IFeeService
    {
        Task<List<Fee>> GetAllAsync();
        Task<Fee?> GetByIdAsync(int id);
        Task CreateAsync(Fee fee);
        Task UpdateAsync(Fee fee);
        Task DeleteAsync(int id);
    }
}
