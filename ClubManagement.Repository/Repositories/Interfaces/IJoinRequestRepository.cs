using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IJoinRequestRepository : IGenericRepository<JoinRequest>
    {
        Task<List<JoinRequest>> GetByUserAsync(int userId);
        Task<List<JoinRequest>> GetByClubAsync(int clubId);
    }
}
