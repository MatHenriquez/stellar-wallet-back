namespace StellarWallet.Application.Dtos.Responses
{
    public class LoggedDto(bool success, string token)
    {
        public bool Success { get; set; } = success;
        public string Token { get; set; } = token;
    }
}
