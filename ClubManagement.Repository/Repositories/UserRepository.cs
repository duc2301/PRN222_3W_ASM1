using ClubManagement.Repository.Basic;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClubManagement.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ClubManagementContext _context) : base(_context)
        {

        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            return user;
        }

        public Task<User> SignUp(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetLeadersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "ClubManager")
                .ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}
