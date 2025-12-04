using AutoMapper;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserResponseDTO>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }
        public async Task<UserResponseDTO?> GetByUsernameAsync(string username)
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(username); 

            if (user == null) return null;

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}
