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
    public interface IJoinRequestService
    {
        Task<IEnumerable<JoinRequest>> GetAllAsync();
        Task<JoinRequest?> GetByIdAsync(int id);
        Task<JoinRequest> SubmitAsync(int userId, int clubId, string? note);
        Task<JoinRequest> ApproveAsync(int requestId, int leaderId);
        Task<JoinRequest> RejectAsync(int requestId, int leaderId, string? reason);
    }
}
