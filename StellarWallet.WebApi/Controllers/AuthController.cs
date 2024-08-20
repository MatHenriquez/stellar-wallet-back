using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _authService.Login(loginDto));
            }
            catch (Exception e)
            {
                if (e.Message == "Invalid credentials")
                    return Unauthorized(e.Message);
                else if (e.Message == "User not found")
                    return NotFound(e.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [Authorize]
        [HttpGet("UserToken")]
        public async Task<IActionResult> UserToken()
        {

            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                return Ok(await _authService.AuthenticateToken(jwt));
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                    return Unauthorized(e.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
