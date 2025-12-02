using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubManagement.Repository.Basic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly ClubManagementContext _context;

        public GenericRepository(ClubManagementContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }
    }
}
