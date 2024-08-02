using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;

namespace StellarWallet.Application.Services
{
    public class AuthService(IJwtService jwtService, IEncryptionService encryptionService, IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;

        public async Task<LoggedDto> Login(LoginDto loginDto)
        {
            User? user = await _unitOfWork.User.GetBy("Email", loginDto.Email) ?? throw new Exception("User not found");
            if (!_encryptionService.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid credentials");

            string createdToken = _jwtService.CreateToken(user.Email, user.Role);

            return new LoggedDto(true, createdToken, user.PublicKey);
        }

        public bool AuthenticateEmail(string jwt, string email)
        {
            string jwtEmail = _jwtService.DecodeToken(jwt) ?? throw new Exception("Unauthorized");

            return jwtEmail.Equals(email);
        }
    }
}
