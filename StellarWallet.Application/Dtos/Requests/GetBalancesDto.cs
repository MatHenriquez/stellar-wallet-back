namespace StellarWallet.Application.Dtos.Requests
{
    public class GetBalancesDto
    {
        public string PublicKey { get; set; } = string.Empty;
        public bool FilterZeroBalances { get; set; } = false;

        public GetBalancesDto() { }
    }
}
