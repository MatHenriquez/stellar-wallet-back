using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public StellarAccount CreateAccount();
        public Task<bool> SendPayment(SendPaymentDto sendPaymentDto);
    }
}
