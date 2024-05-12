namespace StellarWallet.Application.Interfaces
{
    public interface IJwtService
    {
        string CreateToken(string email, string role);
        string DecodeToken(string token);
    }
}
