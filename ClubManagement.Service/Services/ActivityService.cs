using AutoMapper;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Service.DTOs.RequestDTOs.Activity;
using Activity = ClubManagement.Repository.Models.Activity;

namespace ClubManagement.Service.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;
        
        public ActivityService(IUnitOfWork unitOfWork, IMapper mapper, IActivityRepository activityRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityRepository = activityRepository;
        }

        public async Task<List<ActivityResponseDTO>> GetAllAsync()
        {
            var activities = await _activityRepository.GetAllActivitiesWithRelations();
            return _mapper.Map<List<ActivityResponseDTO>>(activities);
        }

        public async Task<ActivityResponseDTO?> GetByIdAsync(int id)
        {
            var activity = await _activityRepository.GetActivityWithRelationsById(id);
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
            var activity = await _activityRepository.GetActivityWithRelationsById(id);            
            if (activity == null)
            {
                throw new Exception($"Activity with ID {id} not found");
            }
            foreach (var participant in activity.ActivityParticipants.ToList())
            {
                _unitOfWork.ActivityParticipantRepository.Remove(participant);
            }
            _unitOfWork.ActivityRepository.Remove(activity);
            
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ActivityResponseDTO>(activity);
        }
    }
}
