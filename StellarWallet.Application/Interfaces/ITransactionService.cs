using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public BlockchainAccount CreateAccount();
        public Task<bool> SendPayment(SendPaymentDto sendPaymentDto);
        public Task<BlockchainPayment[]> GetTransaction(string jwt, int pageNumber, int pageSize);
        public Task<bool> GetTestFunds(string publicKey);
    }
}
