using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Errors;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;

        [HttpPost("AccountCreation")]
        [Authorize]
        public async Task<IActionResult> CreateAccount()
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");

                return Ok(_transactionService.CreateAccount(jwt));
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("Payment")]
        [Authorize]
        public async Task<IActionResult> SendPayment([FromBody] SendPaymentDto sendPaymentDto)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                var paymentResult = await _transactionService.SendPayment(sendPaymentDto, jwt);

                if (paymentResult.IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    int statusCode = paymentResult.Error.ErrorType switch
                    {
                        ErrorType.NotFound => StatusCodes.Status404NotFound,
                        ErrorType.Invalid => StatusCodes.Status400BadRequest,
                        ErrorType.Conflict => StatusCodes.Status409Conflict,
                        ErrorType.ExternalServiceError => StatusCodes.Status503ServiceUnavailable,
                        ErrorType.UnauthorizedError => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    return StatusCode(statusCode, paymentResult.Error.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "There was an error processing the payment.");
            }
        }

        [HttpGet("Payment")]
        [Authorize]
        public async Task<IActionResult> GetPayments([FromQuery] int pageNumber, int pageSize)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");

                return Ok(await _transactionService.GetTransaction(jwt, pageNumber, pageSize));
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("TestFund")]
        [Authorize]
        public async Task<IActionResult> GetTestFunds([FromBody] GetTestFundsDto getTestFundsDto)
        {
            string? jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
                return Unauthorized();

            var getFundsResponse = await _transactionService.GetTestFunds(getTestFundsDto.PublicKey);

            bool wasFunded = getFundsResponse.IsSuccess;

            if (wasFunded)
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Funds not received");
        }

        [HttpGet("Balance")]
        [Authorize]
        public async Task<IActionResult> GetBalances([FromQuery] GetBalancesDto getBalancesDto)
        {
            string? jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
                return Unauthorized();

            try
            {
                return Ok(await _transactionService.GetBalances(getBalancesDto));
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

