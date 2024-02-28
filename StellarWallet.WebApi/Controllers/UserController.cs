using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet(Name = "Users")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _userService.GetAll();
        }

        // Aquí puedes agregar tus acciones...
    }
}
