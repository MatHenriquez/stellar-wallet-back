using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Domain.Repositories
{
    public interface IBlockchainService
    {
        public StellarAccount CreateAccount();
        public Task<bool> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount);

        public Task<BlockchainPayment[]> GetPayments(string publicKey);
    }
}
