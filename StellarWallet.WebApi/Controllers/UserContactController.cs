﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAll(int id)
        {
            try
            {
                return Ok(await _userContactService.GetAll(id));
            }
            catch (Exception e)
            {
                if (e.Message == "User contact not found")
                    return NotFound(e.Message);
                else
                    return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
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
        public async Task<IActionResult> Post(AddContactDto userContact)
        {
            try
            {
                await _userContactService.Add(userContact);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPut()]
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