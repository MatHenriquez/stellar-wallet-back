using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;
using StellarWallet.Infrastructure.DatabaseConnection;

namespace StellarWallet.Infrastructure.Repositories
{
    public class UserRepository(DatabaseContext context) : IUserRepository
    {
        private readonly DatabaseContext _context = context;

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            User? foundUser = await GetById(id);

            _context.Remove(foundUser);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            User? foundUser = await _context.Users.FindAsync(id);
            return foundUser is null ? throw new Exception("User not found") : foundUser;
        }

        public async Task<User?> GetBy(string paramName, string paramValue)
        {
            var propertyInfo = typeof(User).GetProperty(paramName) ?? throw new ArgumentException($"Invalid property: '{paramName}'.");
            var query = _context.Users.Where(u => EF.Property<string>(u, propertyInfo.Name) == paramValue);

            try { 
                return await query.FirstAsync(); 
            }
            catch { 
                return null;
            }
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
