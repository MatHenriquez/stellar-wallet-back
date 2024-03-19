using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id);
        Task<LoggedDto> Add(UserCreationDto user);
        Task Update(UserUpdateDto user);
        Task Delete(int id);
    }
}
