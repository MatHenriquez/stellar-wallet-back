using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<Result<BlockchainAccount, DomainError>> CreateAccount(string jwt);
        public Task<Result<bool, DomainError>> SendPayment(SendPaymentDto sendPaymentDto, string jwt);
        public Task<Result<BlockchainPayment[], DomainError>> GetTransaction(string jwt, int pageNumber, int pageSize);
        public Task<Result<bool, DomainError>> GetTestFunds(string publicKey);
        public Task<Result<FoundBalancesDto, DomainError>> GetBalances(GetBalancesDto publicKey);
    }
}
