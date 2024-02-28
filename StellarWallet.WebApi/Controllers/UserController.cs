using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;

namespace StellarWallet.WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet()]
        public async Task<IEnumerable<User>> Get()
        {
            return await _userService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            return await _userService.GetById(id);
        }

        [HttpPost()]
        public async Task Post(User user)
        {
            await _userService.Add(user);
        }

        [HttpPut()]
        public async Task Put(User user)
        {
            await _userService.Update(user);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _userService.Delete(id);
        }
    }
}
