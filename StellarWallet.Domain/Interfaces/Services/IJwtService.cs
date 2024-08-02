namespace StellarWallet.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        string CreateToken(string email, string role);
        string DecodeToken(string token);
    }
}
