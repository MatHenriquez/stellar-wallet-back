using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StellarWallet.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StellarWallet.Application.Services
{
    public class JwtService(IConfiguration config) : IJwtService
    {
        private readonly string? secretKey = config.GetSection("settings").GetSection("secretKey").ToString();

        public string CreateToken(string email)
        {
            if (secretKey == null)
                throw new Exception("Secret key not found");

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenConfig);
        }

        public string DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (secretKey == null)
                throw new Exception("Secret key not found");
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var allClaims = claimsPrincipal.Claims.ToList();

                if (allClaims.Count == 0)
                    throw new Exception("No claims found");
                return allClaims[0].Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error decoding JWT: {ex.Message}");
            }
        }
    }
}
