using stellar_dotnet_sdk;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public StellarAccount CreateAccount();
        public Task<bool> SendPayment(SendPaymentDto sendPaymentDto);

        public Task<BlockchainPayment[]> GetTransaction(string jwt);
    }
}
