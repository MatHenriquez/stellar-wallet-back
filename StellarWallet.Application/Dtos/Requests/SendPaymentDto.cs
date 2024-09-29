using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests
{
    public class SendPaymentDto(string destinationPublicKey, decimal amount, string memo)
    {
        [Required(ErrorMessage = "Public key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
        public string DestinationPublicKey { get; set; } = destinationPublicKey;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Invalid amount")]
        public decimal Amount { get; set; } = amount;

        [StringLength(28, MinimumLength = 1, ErrorMessage = "Memo must have between 1 and 28 characters")]
        public string? Memo { get; set; } = memo;
    }
}
