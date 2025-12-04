using ClubManagement.Repository.Basic;
using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Repositories;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Repository.UnitOfWork.Interface;

namespace ClubManagement.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClubManagementContext _context;

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        private IPaymentRepository _paymentRepository;
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

        private IMembershipRepository _membershipRepository;
        public IMembershipRepository MembershipRepository => _membershipRepository ??= new MembershipRepository(_context);

        private IJoinRequestRepository _joinRequestRepository;
        public IJoinRequestRepository JoinRequestRepository => _joinRequestRepository ??= new JoinRequestRepository(_context);

        private IFeeRepository _feeRepository;
        public IFeeRepository FeeRepository => _feeRepository ??= new FeeRepository(_context);

        private IClubRepository _clubRepository;
        public IClubRepository ClubRepository => _clubRepository ??= new ClubRepository(_context);

        private IActivityRepository _activityRepository;
        public IActivityRepository ActivityRepository => _activityRepository ??= new ActivityRepository(_context);

        private IActivityParticipantRepository _activityParticipantRepository;
        public IActivityParticipantRepository ActivityParticipantRepository => _activityParticipantRepository ??= new ActivityParticipantRepository(_context);




        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // IDisposable Implementation
        private bool disposed = false;

        public UnitOfWork(ClubManagementContext context)
        {
            _context = context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ClubManagementContext DbContext => _context;
    }

}
