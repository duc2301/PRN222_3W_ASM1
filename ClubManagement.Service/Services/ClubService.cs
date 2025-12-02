using AutoMapper;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;


namespace ClubManagement.Service.Services
{
    public class ClubService : IClubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClubService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateClubRequestDTO> CreateAsync(CreateClubRequestDTO club)
        {
            var clubEntity = _mapper.Map<Club>(club);
            await _unitOfWork.ClubRepository.CreateAsync(clubEntity);
            await _unitOfWork.SaveChangesAsync();
            return club;
        }

        public async Task<ClubResponseDTO> DeleteAsync(int id)
        {
            var clubrequest = await _unitOfWork.ClubRepository.GetByIdAsync(id);
            _unitOfWork.ClubRepository.Remove(clubrequest);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClubResponseDTO>(clubrequest);
        }

        public async Task<List<ClubResponseDTO>> GetAllAsync()
        {
            var clubs = await _unitOfWork.ClubRepository.GetAllAsync();
            return _mapper.Map<List<ClubResponseDTO>>(clubs);
        }

        public async Task<ClubResponseDTO?> GetByIdAsync(int id)
        {
            var clubrequest = await _unitOfWork.ClubRepository.GetByIdAsync(id);
            return _mapper.Map<ClubResponseDTO>(clubrequest);
        }

        public async Task<UpdateClubRequestDTO> UpdateAsync(UpdateClubRequestDTO club)
        {
            var clubrequest = _mapper.Map<Club>(club);
            _unitOfWork.ClubRepository.Update(clubrequest);
            await _unitOfWork.SaveChangesAsync();
            return club;
        }
    }
}
