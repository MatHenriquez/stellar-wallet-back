using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class AuthService(IUserRepository userRepository) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<LoggedDto> Login(LoginDto loginDto)
        {
            User user = await _userRepository.GetBy("Email", loginDto.Email);

            if (user.Password != loginDto.Password)
                throw new Exception("Invalid password");

            return new LoggedDto(true, "token");
        }
    }
}
