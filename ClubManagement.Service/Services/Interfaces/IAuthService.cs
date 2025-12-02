using ClubManagement.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<User> Login(string username, string password);
        Task<User> SignUp(string username, string password);
    }
}
