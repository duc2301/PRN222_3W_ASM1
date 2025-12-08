using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IClubService
    {
        Task<List<ClubResponseDTO>> GetAllAsync();
        
        Task<ClubResponseDTO?> GetByIdAsync(int id);
        Task<CreateClubRequestDTO> CreateAsync(CreateClubRequestDTO club);
        Task<UpdateClubRequestDTO> UpdateAsync(UpdateClubRequestDTO club);
        Task<ClubResponseDTO> DeleteAsync(int id);
        Task<ClubMembersPageViewModel?> GetMembersPageAsync(
            int clubId,
            string? search,
            string? roleFilter,
             string? statusFilter,
            int page,
            int pageSize);
        Task<List<ClubResponseDTO>> GetClubsByUsernameAsync(string userName);
    }
}
