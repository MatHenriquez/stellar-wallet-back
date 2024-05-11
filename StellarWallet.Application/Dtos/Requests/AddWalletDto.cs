using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Application.Dtos.Requests
{
    public class AddWalletDto(string PublicKey, string SecretKey)
    {
        [Required(ErrorMessage = "Public key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
        public string PublicKey { get; set; } = PublicKey;

        [Required(ErrorMessage = "Secret key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Secret key must have 56 characters")]
        public string SecretKey { get; set; } = SecretKey;
    }
}
