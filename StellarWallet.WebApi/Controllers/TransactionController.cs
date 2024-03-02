using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;

        [HttpGet("AccountCreation")]
        public IActionResult CreateAccount()
        {
            return Ok(_transactionService.CreateAccount());
        }

        [HttpPost("Payment")]
        public async Task<IActionResult> SendPayment([FromBody] SendPaymentDto sendPaymentDto)
        {
            try
            {
                await _transactionService.SendPayment(sendPaymentDto);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
