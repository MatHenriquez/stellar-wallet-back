using StellarWallet.Domain.Entities;

namespace StellarWallet.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetBy(string criteria, string value);
        Task Add(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
