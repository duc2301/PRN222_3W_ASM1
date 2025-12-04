using AutoMapper;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClubManagement.Service.DTOs.RequestDTOs.Activity;
using Activity = ClubManagement.Repository.Models.Activity;

namespace ClubManagement.Service.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ActivityResponseDTO>> GetAllAsync()
        {
            var activities = await _unitOfWork.ActivityRepository.GetAllAsync();
            return _mapper.Map<List<ActivityResponseDTO>>(activities);
        }

        public async Task<ActivityResponseDTO?> GetByIdAsync(int id)
        {
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(id);
            return _mapper.Map<ActivityResponseDTO>(activity);
        }

        public async Task<ActivityResponseDTO> CreateAsync(ActivityCreateDTO activity)
        {
            var activityEntity = _mapper.Map<Activity>(activity);
            var createdActivity = await _unitOfWork.ActivityRepository.CreateAsync(activityEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ActivityResponseDTO>(createdActivity);
        }

        public async Task<ActivityResponseDTO> UpdateAsync(ActivityUpdateDTO activity)
        {
            var activityEntity = _mapper.Map<Activity>(activity);
            _unitOfWork.ActivityRepository.Update(activityEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ActivityResponseDTO>(activityEntity);
        }

        public async Task<ActivityResponseDTO> DeleteAsync(int id)
        {
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(id);
            _unitOfWork.ActivityRepository.Remove(activity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ActivityResponseDTO>(activity);
        }
    }
}
