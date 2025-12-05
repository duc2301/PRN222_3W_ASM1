using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<List<Club>> GetAllAsync();
        Task<Club> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}
