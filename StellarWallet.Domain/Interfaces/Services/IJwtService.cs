using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        Result<string, DomainError> CreateToken(string email, string role);
        Result<string, DomainError> DecodeToken(string token);
    }
}
