using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Application.Dtos.Responses
{
    public class BlockchainAccountDto
    {
        public string? PublicKey { get; set; }
        public string? SecretKey { get; set; }
    }
}
