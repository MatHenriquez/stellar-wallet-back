using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Dtos.Responses
{
    public class FoundBalancesDto(AccountBalances[] balances, int totalPages)
    {
        public AccountBalances[] Balances { get; set; } = balances;
        public int TotalPages { get; set; } = totalPages;
    }
}
