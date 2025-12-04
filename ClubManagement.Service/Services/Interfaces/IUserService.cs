using ClubManagement.Service.DTOs.ResponseDTOs;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllAsync();
        Task<List<UserResponseDTO>> GetLeadersAsync();
    }
}
