using AutoMapper;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services;
using ClubManagement.Service.Services.Interfaces;


namespace ClubManagement.Service.ServiceProviders
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceProviders(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IAuthService AuthService => new AuthService(_unitOfWork, _mapper);

        public IClubService ClubService => new ClubService(_unitOfWork, _mapper);

        public IUserService UserService => new UserService(_unitOfWork, _mapper);
    }
}
