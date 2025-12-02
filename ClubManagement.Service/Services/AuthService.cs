using AutoMapper;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<User> Login(string username, string password)
        {
            return await _unitOfWork.UserRepository.Login(username, password);
        }

        public Task<User> SignUp(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
