using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.Models;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> Login(string username, string password);
        Task<User> SignUp(string username, string password);
        Task<List<User>> GetLeadersAsync();
        Task<User?> GetByUsernameAsync(string username);
    }
}
