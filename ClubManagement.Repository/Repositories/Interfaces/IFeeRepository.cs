using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IFeeRepository : IGenericRepository<Fee>
    {
        Task<List<Fee>> GetByClubAsync(int clubId);
        Task<List<Fee>> GetAllWithClubAsync();
        Task<Fee> GetByIdWithClubAsync(int id);
    }
}
