using AutoMapper;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ClubManagement.Service.ServiceProviders
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ClubManagementContext _context;

        public ServiceProviders(IUnitOfWork unitOfWork, IMapper mapper, ClubManagementContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public IAuthService AuthService => new AuthService(_unitOfWork, _mapper);

        public IClubService ClubService => new ClubService(_unitOfWork, _mapper);

        public IUserService UserService => new UserService(_unitOfWork, _mapper);
        public IJoinRequestService JoinRequestService
     => new JoinRequestService(
         _unitOfWork.JoinRequestRepository,
         _unitOfWork.MembershipRepository,
         _unitOfWork.UserRepository,
         _unitOfWork.ClubRepository,
         _context);
        public IPaymentService PaymentService
        => new PaymentService(
            _unitOfWork.PaymentRepository,
            _unitOfWork.DbContext,
            _mapper
        );

        public IFeeService FeeService => new FeeService(_unitOfWork, _mapper);
    }
}
