using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Utilities;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IJwtService jwtService, IAuthService authService, IUnitOfWork unitOfWork) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;

        public async Task<Result<BlockchainAccount, DomainError>> CreateAccount(string jwt)
        {
            var userEmail = _jwtService.DecodeToken(jwt);

            User? user = await _unitOfWork.User.GetBy("Email", userEmail.Value);
            if (user is null)
            {
                return Result<BlockchainAccount, DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            return Result<BlockchainAccount, DomainError>.Success(_blockchainService.CreateAccount(user.Id));
        }

        public async Task<Result<bool, DomainError>> SendPayment(SendPaymentDto sendPaymentDto, string jwt)
        {
            var userEmail = _jwtService.DecodeToken(jwt);
            User? user = await _unitOfWork.User.GetBy("Email", userEmail.Value);

            if (user is null)
            {
                return Result<bool, DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            var isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, userEmail.Value);
            if (!isAnAuthorizedUser.IsSuccess)
            {
                return Result<bool, DomainError>.Failure(DomainError.Unauthorized("Unauthorized"));
            }

            var transactionResponse = await _blockchainService.SendPayment(user.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString(), sendPaymentDto.AssetIssuer, sendPaymentDto.AssetCode, sendPaymentDto.Memo ?? String.Empty);

            bool transactionCompleted = transactionResponse.IsSuccess;

            if (!transactionCompleted)
            {
                return transactionResponse;
            }

            return Result<bool, DomainError>.Success(transactionCompleted);
        }

        public async Task<Result<BlockchainPayment[], DomainError>> GetTransaction(string jwt, int pageNumber, int pageSize)
        {
            var userEmailDecoding = _jwtService.DecodeToken(jwt);

            if (!userEmailDecoding.IsSuccess)
            {
                return Result<BlockchainPayment[], DomainError>.Failure(DomainError.Unauthorized("Unauthorized"));
            }

            User? user = await _unitOfWork.User.GetBy("Email", userEmailDecoding.Value);

            if (user is null)
            {
                return Result<BlockchainPayment[], DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            var isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, userEmailDecoding.Value);
            if (!isAnAuthorizedUser.IsSuccess)
            {
                return Result<BlockchainPayment[], DomainError>.Failure(DomainError.Unauthorized("Unauthorized"));
            }

            var getPaymentsResponse = await _blockchainService.GetPayments(user.PublicKey);

            if (!getPaymentsResponse.IsSuccess)
            {
                return getPaymentsResponse;
            }

            BlockchainPayment[] allPayments = getPaymentsResponse.Value;

            BlockchainPayment[] paginatedPayments = allPayments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Result<BlockchainPayment[], DomainError>.Success(paginatedPayments);
        }

        public async Task<Result<bool, DomainError>> GetTestFunds(string publicKey)
        {
            var getTestFundsResponse = await _blockchainService.GetTestFunds(publicKey);
            if (!getTestFundsResponse.IsSuccess) { return getTestFundsResponse; }

            return Result<bool, DomainError>.Success(getTestFundsResponse.Value);
        }

        public async Task<Result<FoundBalancesDto, DomainError>> GetBalances(GetBalancesDto getBalancesDto)
        {
            var getBalancesResponse = await _blockchainService.GetBalances(getBalancesDto.PublicKey);
            if (!getBalancesResponse.IsSuccess) { return Result<FoundBalancesDto, DomainError>.Failure(getBalancesResponse.Error); }

            List<AccountBalances> balances = getBalancesResponse.Value;

            if (getBalancesDto.FilterZeroBalances)
            {
                balances = balances.Where(balance => balance.Amount != "0.0000000").ToList();
            }

            int totalPages = Paginate.GetTotalPages(balances.Count, getBalancesDto.PageSize);
            var paginatedBalances = Paginate.PaginateQuery<AccountBalances>(balances, getBalancesDto.PageNumber, getBalancesDto.PageSize).ToList();

            return Result<FoundBalancesDto, DomainError>.Success(new FoundBalancesDto(paginatedBalances, totalPages));
        }
    }
}
