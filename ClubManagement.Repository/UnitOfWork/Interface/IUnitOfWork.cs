using ClubManagement.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Repository.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IMembershipRepository MembershipRepository { get; }
        IJoinRequestRepository JoinRequestRepository { get; }
        IFeeRepository FeeRepository { get; }
        IClubRepository ClubRepository { get; }
        IActivityRepository ActivityRepository { get; }
        IActivityParticipantRepository ActivityParticipantRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
