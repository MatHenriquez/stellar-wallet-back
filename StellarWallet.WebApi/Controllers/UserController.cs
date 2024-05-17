using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet()]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");

                return Ok(await _userService.GetById(id, jwt));
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized") return Unauthorized();
                else if (e.Message == "User not found") return NotFound(e.Message);
                else return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Post(UserCreationDto user)
        {
            try
            {
                return Ok(await _userService.Add(user));
            }
            catch (Exception e)
            {
                if (e.Message == "User already exists")
                    return BadRequest(e.Message);
                else
                    return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> Put(UserUpdateDto user)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                await _userService.Update(user, jwt);
                return Ok();
            }
            catch (Exception e) {
                if (e.Message == "Unauthorized") return Unauthorized();
                else if (e.Message == "User not found") return NotFound(e.Message);
                else return StatusCode(500, $"Error: {e.Message}");
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                await _userService.Delete(id, jwt);
                return Ok();
            }
            catch (Exception e) {
                if (e.Message == "Unauthorized") return Unauthorized();
                else if (e.Message == "User not found") return NotFound(e.Message);
                else return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPost("wallet")]
        [Authorize]
        public async Task<IActionResult> AddWallet([FromBody] AddWalletDto wallet)
        {
            try
            {
                string? jwt = await HttpContext.GetTokenAsync("access_token");
                if (jwt is null)
                    return Unauthorized();
                await _userService.AddWallet(wallet, jwt);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Error adding wallet: Wallet already exists")
                    return Conflict(e.Message);
                else if (e.Message == "Error adding wallet: User already has 5 wallets")
                    return Conflict(e.Message);
                else
                    return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
