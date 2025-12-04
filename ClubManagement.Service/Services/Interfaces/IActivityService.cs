using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClubManagement.Service.DTOs.RequestDTOs.Activity;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityResponseDTO>> GetAllAsync();
        Task<ActivityResponseDTO?> GetByIdAsync(int id);
        Task<ActivityResponseDTO> CreateAsync(ActivityCreateDTO activity);
        Task<ActivityResponseDTO> UpdateAsync(ActivityUpdateDTO activity);
        Task<ActivityResponseDTO> DeleteAsync(int id);
    }
}
