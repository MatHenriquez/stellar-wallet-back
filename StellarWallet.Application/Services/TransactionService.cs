using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IUserRepository userRepository) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IUserRepository _userService = userRepository;

        public StellarAccount CreateAccount()
        {
            return _blockchainService.CreateAccount();
        }

        public async Task<bool> SendPayment(SendPaymentDto sendPaymentDto)
        {
            User user = await _userService.GetById(sendPaymentDto.UserId);
            bool transactionCompleted = await _blockchainService.SendPayment(user.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString());

            if (transactionCompleted) 
            {
                return transactionCompleted;
            } else
            {
                throw new Exception("Transaction failed");
            }
        }
    }
}
