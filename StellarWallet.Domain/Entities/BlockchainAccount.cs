using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Domain.Entities
{
    public class BlockchainAccount(string PublicKey, string SecretKey)
    {
        [Required(ErrorMessage = "Public key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
        public string PublicKey { get; set; } = PublicKey;

        [Required(ErrorMessage = "Secret key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Secret key must have 56 characters")]
        public string SecretKey { get; set; } = SecretKey;
    }
}
