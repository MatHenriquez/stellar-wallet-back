using StellarWallet.Domain.Entities;

namespace StellarWallet.Domain.Interfaces
{
    public interface IBlockchainAccountRepository
    {
        public Task Add(BlockchainAccount blockchainAccount);
    }
}
