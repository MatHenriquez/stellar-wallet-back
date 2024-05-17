using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<BlockchainAccount> CreateAccount(string jwt);
        public Task<bool> SendPayment(SendPaymentDto sendPaymentDto, string jwt);
        public Task<BlockchainPayment[]> GetTransaction(string jwt, int pageNumber, int pageSize);
        public Task<bool> GetTestFunds(string publicKey);
        public Task<FoundBalancesDto> GetBalances(GetBalancesDto publicKey);
    }
}
