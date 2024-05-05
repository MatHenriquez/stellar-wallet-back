using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Dtos.Responses
{
    public class FoundBalancesDto(AccountBalances[] balances)
    {
        public AccountBalances[] Balances { get; set; } = balances;
    }
}
