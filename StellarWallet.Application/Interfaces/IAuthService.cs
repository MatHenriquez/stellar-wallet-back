using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;

namespace StellarWallet.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoggedDto> Login(LoginDto loginDto);
        bool AuthenticateEmail(string jwt, string email);
    }
}
