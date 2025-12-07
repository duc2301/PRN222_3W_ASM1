using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IFeeService
    {
        Task<FeeResponseDTO> CreateAsync(CreateFeeRequestDTO dto);
        Task<IEnumerable<FeeResponseDTO>> GetByClubAsync(int clubId);
        Task<FeeResponseDTO> GetByIdAsync(int feeId);
        Task<IEnumerable<FeeResponseDTO>> GetAllAsync();
    }
}

