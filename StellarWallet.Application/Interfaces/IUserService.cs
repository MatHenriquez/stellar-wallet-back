using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<Result<UserDto, DomainError>> GetById(int id, string jwt);
        Task<Result<LoggedDto, DomainError>> Add(UserCreationDto user);
        Task<Result<bool, DomainError>> Update(UserUpdateDto user, string jwt);
        Task<Result<bool, DomainError>> Delete(int id, string jwt);
        Task<Result<bool, DomainError>> AddWallet(AddWalletDto wallet, string jwt);
    }
}
