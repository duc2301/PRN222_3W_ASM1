using AutoMapper;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class FeeService : IFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FeeResponseDTO> CreateAsync(CreateFeeRequestDTO dto)
        {
            var fee = new Fee
            {
                ClubId = dto.ClubId,
                Title = dto.Title,
                Amount = dto.Amount,
                DueDate = dto.DueDate,
                Description = dto.Description,
                CreatedAt = System.DateTime.Now
            };

            await _unitOfWork.FeeRepository.CreateAsync(fee);
            await _unitOfWork.SaveChangesAsync();

            // Lấy tất cả active members của club
            var members = await _unitOfWork.MembershipRepository.GetActiveMembersByClubIdAsync(dto.ClubId);
            
            // Tính số tiền chia đều cho mỗi member
            if (members.Any())
            {
                decimal amountPerMember = dto.Amount / members.Count;
                
                // Tạo Payment cho mỗi member với status "Unpaid" (chưa đóng)
                foreach (var member in members)
                {
                    var payment = new Payment
                    {
                        FeeId = fee.FeeId,
                        UserId = member.UserId,
                        Amount = amountPerMember,
                        Status = "Unpaid", // Mặc định là chưa đóng
                        PaymentDate = null // Chưa đóng nên chưa có ngày
                    };
                    
                    await _unitOfWork.PaymentRepository.CreateAsync(payment);
                }
                
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<FeeResponseDTO>(fee);
        }

        public async Task<IEnumerable<FeeResponseDTO>> GetByClubAsync(int clubId)
        {
            var fees = await _unitOfWork.FeeRepository.GetByClubAsync(clubId);
            return _mapper.Map<IEnumerable<FeeResponseDTO>>(fees);
        }

        public async Task<FeeResponseDTO> GetByIdAsync(int feeId)
        {
            var fee = await _unitOfWork.FeeRepository.GetByIdWithClubAsync(feeId);
            if (fee == null) return null;
            
            return _mapper.Map<FeeResponseDTO>(fee);
        }

        public async Task<IEnumerable<FeeResponseDTO>> GetAllAsync()
        {
            var fees = await _unitOfWork.FeeRepository.GetAllWithClubAsync();
            return _mapper.Map<IEnumerable<FeeResponseDTO>>(fees);
        }

        public async Task<List<FeeResponseDTO>> GetAvailableFeesAsync(string userName)
        {
            var fees = await _unitOfWork.FeeRepository.GetAvailableFeesAsync(userName);
            return _mapper.Map<List<FeeResponseDTO>>(fees);
        }
    }
}

