namespace StellarWallet.Application.Dtos.Responses
{
    public class UserContactsDto(string alias, string publicKey)
    {
        public string Alias { get; set; } = alias;
        public string PublicKey { get; set; } = publicKey;
    }
}
