using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id, string jwt);
        Task<LoggedDto> Add(UserCreationDto user);
        Task Update(UserUpdateDto user, string jwt);
        Task Delete(int id, string jwt);
        Task AddWallet(AddWalletDto wallet, string jwt);
    }
}
