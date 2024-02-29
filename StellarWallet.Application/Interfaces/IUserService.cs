using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id);
        Task Add(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
