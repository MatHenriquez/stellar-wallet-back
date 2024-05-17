using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserContactController(IUserContactService userContactService) : ControllerBase
    {
        private readonly IUserContactService _userContactService = userContactService;

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAll(int id)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                return Ok(await _userContactService.GetAll(id, jwt));
            }
            catch (Exception e)
            {
                if (e.Message == "User contact not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();
                else
                    return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userContactService.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> Post(AddContactDto userContact)
        {
            try
            {
                string? jwt = await HttpContext.GetTokenAsync("access_token");
                if (jwt is null)
                    return Unauthorized();

                await _userContactService.Add(userContact, jwt);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == "Contact already exists")
                    return Conflict(e.Message);
                else if (e.Message == "User has reached the maximum number of contacts")
                    return Conflict(e.Message);
                else
                    return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> Put(UpdateContactDto userContact)
        {
            try
            {
                await _userContactService.Update(userContact);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
