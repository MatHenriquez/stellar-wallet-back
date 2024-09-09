using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Errors;

namespace StellarWallet.Domain.Interfaces.Services
{
    public interface IBlockchainService
    {
        public BlockchainAccount CreateAccount(int userId);
        public AccountKeyPair CreateKeyPair();
        public Task<Result<bool, DomainError>> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount);
        public Task<Result<BlockchainPayment[], DomainError>> GetPayments(string publicKey);
        public Task<Result<bool, DomainError>> GetTestFunds(string publicKey);
        public Task<Result<List<AccountBalances>, DomainError>> GetBalances(string publicKey);
    }
}
