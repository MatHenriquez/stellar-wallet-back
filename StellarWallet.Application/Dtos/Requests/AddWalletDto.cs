using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Application.Dtos.Requests
{
    public class AddWalletDto(string PublicKey, string SecretKey)
    {
        public string PublicKey { get; set; } = PublicKey;
        public string SecretKey { get; set; } = SecretKey;
    }
}
