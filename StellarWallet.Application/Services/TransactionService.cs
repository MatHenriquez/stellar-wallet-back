using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IJwtService jwtService, IAuthService authService, IUnitOfWork unitOfWork) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;

        private void AuthenticateUserEmail(string jwt, string email)
        {
            bool isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, email);
            if (!isAnAuthorizedUser) throw new Exception("Unauthorized");
        }

        public async Task<BlockchainAccount> CreateAccount(string jwt)
        {
            string userEmail = _jwtService.DecodeToken(jwt);
            User user = await _unitOfWork.User.GetBy("Email", userEmail) ?? throw new Exception("User not found");
            return _blockchainService.CreateAccount(user.Id);
        }

        public async Task<bool> SendPayment(SendPaymentDto sendPaymentDto, string jwt)
        {
            string userEmail = _jwtService.DecodeToken(jwt);
            User user = await _unitOfWork.User.GetBy("Email", userEmail) ?? throw new Exception("User not found");

            AuthenticateUserEmail(jwt, user.Email);

            bool transactionCompleted = await _blockchainService.SendPayment(user.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString());

            if (transactionCompleted)
                return transactionCompleted;
            else
                throw new Exception("Transaction failed");
        }

        public async Task<BlockchainPayment[]> GetTransaction(string jwt, int pageNumber, int pageSize)
        {
            string userEmail = _jwtService.DecodeToken(jwt);
            User user = await _unitOfWork.User.GetBy("Email", userEmail) ?? throw new Exception("User not found");

            AuthenticateUserEmail(jwt, user.Email);

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

            int pageSize = getBalancesDto.PageSize > balances.Length ? balances.Length : getBalancesDto.PageSize;

            balances = balances.Skip((pageSize * getBalancesDto.PageNumber) - 1).Take(pageSize).ToArray();

            int totalPages = (int)Math.Ceiling((double)balances.Length / getBalancesDto.PageSize);

            totalPages = totalPages == 0 ? 1 : totalPages;

            return new FoundBalancesDto(balances, totalPages);
        }
    }
}
