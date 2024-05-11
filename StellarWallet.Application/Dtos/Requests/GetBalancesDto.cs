using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests
{
    public class GetBalancesDto
    {
        [Required(ErrorMessage = "Public key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
        public string PublicKey { get; set; } = string.Empty;

        public bool FilterZeroBalances { get; set; } = false;

        public GetBalancesDto() { }
    }
}
