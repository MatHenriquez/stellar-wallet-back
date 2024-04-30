using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IUserRepository userRepository, IJwtService jwtService) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userService = userRepository;

        public StellarAccount CreateAccount()
        {
            return _blockchainService.CreateAccount();
        }

        public async Task<bool> SendPayment(SendPaymentDto sendPaymentDto)
        {
            string userEmail = _jwtService.DecodeToken(sendPaymentDto.UserToken);
            User user = await _userService.GetBy("Email", userEmail) ?? throw new Exception("User not found");

            bool transactionCompleted = await _blockchainService.SendPayment(user.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString());

            if (transactionCompleted)
                return transactionCompleted;
            else
                throw new Exception("Transaction failed");
        }
    }
}
