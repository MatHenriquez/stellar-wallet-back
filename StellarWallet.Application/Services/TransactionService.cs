using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IUserRepository userRepository, IJwtService jwtService) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userService = userRepository;

        public BlockchainAccount CreateAccount()
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

        public async Task<BlockchainPayment[]> GetTransaction(string jwt, int pageNumber, int pageSize)
        {
            string userEmail = _jwtService.DecodeToken(jwt);
            User user = await _userService.GetBy("Email", userEmail) ?? throw new Exception("User not found");

            BlockchainPayment[] allPayments = await _blockchainService.GetPayments(user.PublicKey);

            BlockchainPayment[] paginatedPayments = allPayments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return paginatedPayments;
        }

        public async Task<bool> GetTestFunds(string publicKey)
        {
            return await _blockchainService.GetTestFunds(publicKey);
        }

        public async Task<FoundBalancesDto> GetBalances(GetBalancesDto getBalancesDto)
        {
            AccountBalances[] balances = await _blockchainService.GetBalances(getBalancesDto.PublicKey);

            if (getBalancesDto.FilterZeroBalances)
                balances = balances.Where(balance => balance.Amount == "0").ToArray();

            return new FoundBalancesDto(balances);
        }
    }
}
