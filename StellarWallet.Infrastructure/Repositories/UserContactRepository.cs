using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces;
using StellarWallet.Infrastructure.DatabaseConnection;


namespace StellarWallet.Infrastructure.Repositories
{
    public class UserContactRepository(DatabaseContext context) : IUserContactRepository
    {
        private readonly DatabaseContext _context = context;

        public async Task Add(UserContact userContact)
        {
            await _context.UserContacts.AddAsync(userContact);
        }

        public async Task Delete(int id)
        {
            UserContact? foundUserContact = await GetById(id) ?? throw new Exception("User contact not found");
            _context.UserContacts.Remove(foundUserContact);
            await _context.SaveChangesAsync();
        }

        public async Task<UserContact> GetById(int id)
        {
            UserContact? foundUserContact = await _context.UserContacts.FindAsync(id);
            return foundUserContact is null ? throw new Exception("User contact not found") : foundUserContact;
        }

        public async Task<IEnumerable<UserContact>> GetAll(int userId)
        {
            return await _context.UserContacts.Where(uc => uc.UserId == userId).ToListAsync();
        }

        public Task Update(UserContact userContact)
        {
            _context.UserContacts.Update(userContact);
            return _context.SaveChangesAsync();
        }
    }
}
