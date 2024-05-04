namespace StellarWallet.Application.Dtos.Requests
{
    public class GetTestFundsDto(string publicKey)
    {
        public string PublicKey { get; set; } = publicKey;
    }
}
