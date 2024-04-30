using Microsoft.Extensions.Configuration;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Interfaces
{
    public interface IJwtService
    {
        string CreateToken(string email);
        string DecodeToken(string token);
    }
}
