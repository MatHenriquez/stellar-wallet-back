using StellarWallet.Domain.Entities;

namespace StellarWallet.Domain.Interfaces
{
    public interface IUserContactRepository
    {
        Task<IEnumerable<UserContact>> GetAll(int userId);
        Task<UserContact> GetById(int id);
        Task Add(UserContact userContact);
        Task Update(UserContact userContact);
        Task Delete(int id);
    }
}
