using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Services
{
    public class AuthService(IJwtService jwtService, IEncryptionService encryptionService, IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;

        public async Task<Result<LoggedDto, DomainError>> Login(LoginDto loginDto)
        {
            User? user = await _unitOfWork.User.GetBy("Email", loginDto.Email);
            if (user is null)
            {
                return Result<LoggedDto, DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            if (!_encryptionService.Verify(loginDto.Password, user.Password))
            {
                return Result<LoggedDto, DomainError>.Failure(DomainError.Unauthorized("Invalid credentials"));
            }

            var createdTokenResponse = _jwtService.CreateToken(user.Email, user.Role);

            if (!createdTokenResponse.IsSuccess)
            {
                return Result<LoggedDto, DomainError>.Failure(createdTokenResponse.Error);
            }

            var createdToken = createdTokenResponse.Value;

            return Result<LoggedDto, DomainError>.Success(new LoggedDto(createdTokenResponse.IsSuccess, createdToken, user.PublicKey));
        }

        public Result<bool, DomainError> AuthenticateEmail(string jwt, string email)
        {
            var jwtEmailDecoding = _jwtService.DecodeToken(jwt);

            if (!jwtEmailDecoding.IsSuccess)
            {
                return Result<bool, DomainError>.Failure(jwtEmailDecoding.Error);
            }

            var jwtEmail = jwtEmailDecoding.Value;

            return Result<bool, DomainError>.Success(jwtEmail.Equals(email));
        }

        public async Task<Result<bool, DomainError>> AuthenticateToken(string jwt)
        {
            var jwtEmailResponse = _jwtService.DecodeToken(jwt);

            if (!jwtEmailResponse.IsSuccess)
            {
                return Result<bool, DomainError>.Failure(jwtEmailResponse.Error);
            }

            var email = jwtEmailResponse.Value;

            var userExists = await _unitOfWork.User.GetBy("Email", email) is not null;

            return Result<bool, DomainError>.Success(userExists);
        }
    }
}
