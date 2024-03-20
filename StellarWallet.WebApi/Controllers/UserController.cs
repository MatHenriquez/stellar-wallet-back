using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService, IBlockchainService stellarService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IBlockchainService _stellarService = stellarService;

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _userService.GetById(id));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Post(UserCreationDto user)
        {
            StellarAccount account = _stellarService.CreateAccount();
            user.SecretKey = user.SecretKey is null ? account.SecretKey : user.SecretKey;
            user.PublicKey = user.PublicKey is null ? account.PublicKey : user.PublicKey;

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
        public async Task<IActionResult> Put(UserUpdateDto user)
        {
            try
            {
                await _userService.Update(user);
                return Ok();
            }
            catch (Exception e) { return NotFound(e.Message); }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok();
            }
            catch (Exception e) { return NotFound(e.Message); }
        }
    }
}
