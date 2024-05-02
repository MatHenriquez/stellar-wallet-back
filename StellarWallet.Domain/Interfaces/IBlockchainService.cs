using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Domain.Repositories
{
    public interface IBlockchainService
    {
        public StellarAccount CreateAccount();
        public Task<bool> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount);

        public Task<BlockchainPayment[]> GetPayments(string publicKey);
    }
}
