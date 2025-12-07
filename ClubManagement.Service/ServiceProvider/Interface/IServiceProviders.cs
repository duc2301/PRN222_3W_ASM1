using ClubManagement.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.ServiceProviders.Interface
{
    public interface IServiceProviders
    {
        IAuthService AuthService { get; }
        IClubService ClubService { get; }
        IUserService UserService { get; }
        IJoinRequestService JoinRequestService { get; }
        IPaymentService PaymentService { get; }
        IFeeService FeeService { get; }
    }
}
