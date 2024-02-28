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
            User? foundUser = await _context.Users.FindAsync(id);
            _context.Remove(foundUser);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
