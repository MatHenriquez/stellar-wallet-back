using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoggedDto, DomainError>> Login(LoginDto loginDto);
        Result<bool, DomainError> AuthenticateEmail(string jwt, string email);
        Task<Result<bool, DomainError>> AuthenticateToken(string jwt);
    }
}
