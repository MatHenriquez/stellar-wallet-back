using Moq;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Structs;

namespace StellarWallet.UnitTest.Application.Services
{
    public class TransactionServiceTest
    {
        private readonly Mock<IBlockchainService> _mockBlockchainService = new();
        private readonly Mock<IJwtService> _mockJwtService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IAuthService> _mockAuthService = new();
        private readonly TransactionService _sut;
        private const string JWT = "token";
        private readonly User _validUser = new("John", "Doe", "john.doe@mail.com", "EncryptedPassword", "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA", "SBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA", "admin");

        public TransactionServiceTest()
        {
            _sut = new TransactionService(_mockBlockchainService.Object, _mockJwtService.Object, _mockAuthService.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task When_ValidJwt_Expected_CreateAccount()
        {
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockBlockchainService.Setup(x => x.CreateAccount(_validUser.Id)).Returns(new BlockchainAccount(_validUser.PublicKey, _validUser.SecretKey, _validUser.Id));

            var result = await _sut.CreateAccount(JWT);

            Assert.True(result.PublicKey == _validUser.PublicKey);
            Assert.True(result.SecretKey == _validUser.SecretKey);
            Assert.True(result.UserId == _validUser.Id);
        }

        [Fact]
        public async Task When_UnexistingUser_Expected_ThrowException()
        {
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<Exception>(() => _sut.CreateAccount(JWT));
        }

        [Fact]
        public async Task When_InvalidJwt_Expected_ThrowException()
        {
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockBlockchainService.Setup(x => x.CreateAccount(_validUser.Id)).Returns(new BlockchainAccount(_validUser.PublicKey, _validUser.SecretKey, _validUser.Id));

            await Assert.ThrowsAsync<Exception>(() => _sut.CreateAccount("invalidToken"));
        }

        [Fact]
        public async Task When_ValidSendPayment_Expected_True()
        {
            var sendPaymentDto = new SendPaymentDto(_validUser.PublicKey, 10);
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockAuthService.Setup(x => x.AuthenticateEmail(JWT, _validUser.Email)).Returns(true);
            _mockBlockchainService.Setup(x => x.SendPayment(_validUser.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString())).ReturnsAsync(true);

            var result = await _sut.SendPayment(sendPaymentDto, JWT);

            Assert.True(result);
        }

        [Fact]
        public async Task When_UnexistingUserSendPayment_Expected_ThrowException()
        {
            var sendPaymentDto = new SendPaymentDto(_validUser.PublicKey, 10);
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<Exception>(() => _sut.SendPayment(sendPaymentDto, JWT));
        }

        [Fact]
        public async Task When_InvalidJwtSendPayment_Expected_ThrowException()
        {
            var sendPaymentDto = new SendPaymentDto(_validUser.PublicKey, 10);
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockAuthService.Setup(x => x.AuthenticateEmail(JWT, _validUser.Email)).Returns(true);
            _mockBlockchainService.Setup(x => x.SendPayment(_validUser.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString())).ReturnsAsync(true);

            await Assert.ThrowsAsync<Exception>(() => _sut.SendPayment(sendPaymentDto, "invalidToken"));
        }

        [Fact]
        public async Task When_ValidGetTransaction_Expected_PaginatedPayments()
        {
            BlockchainPayment[] blockchainPayments =
            [
                new BlockchainPayment
                {
                    Id = "1",
                    From = "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA",
                    To = "SBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA",
                    Amount = "10",
                    Asset = "XLM",
                    CreatedAt = DateTime.Now.ToString(),
                    WasSuccessful = true
                }
            ];

            var pageNumber = 1;
            var pageSize = 1;
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockAuthService.Setup(x => x.AuthenticateEmail(JWT, _validUser.Email)).Returns(true);
            _mockBlockchainService.Setup(x => x.GetPayments(_validUser.PublicKey)).ReturnsAsync(blockchainPayments);

            var result = await _sut.GetTransaction(JWT, pageNumber, pageSize);

            Assert.True(result.Length == pageSize);
            Assert.Equal(result[0], blockchainPayments[0]);
        }

        [Fact]
        public async Task When_UnexistingUserGetTransaction_Expected_ThrowException()
        {
            var pageNumber = 1;
            var pageSize = 1;
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<Exception>(() => _sut.GetTransaction(JWT, pageNumber, pageSize));
        }

        [Fact]
        public async Task When_InvalidJwtGetTransaction_Expected_ThrowException()
        {
            var pageNumber = 1;
            var pageSize = 1;
            _mockJwtService.Setup(x => x.DecodeToken(JWT)).Returns(_validUser.Email);
            _mockUnitOfWork.Setup(x => x.User.GetBy("Email", _validUser.Email)).ReturnsAsync(_validUser);
            _mockAuthService.Setup(x => x.AuthenticateEmail(JWT, _validUser.Email)).Returns(true);

            await Assert.ThrowsAsync<Exception>(() => _sut.GetTransaction("invalidToken", pageNumber, pageSize));
        }

        [Fact]
        public async Task When_ValidGetTestFunds_Expected_True()
        {
            _mockBlockchainService.Setup(x => x.GetTestFunds(_validUser.PublicKey)).ReturnsAsync(true);

            var result = await _sut.GetTestFunds(_validUser.PublicKey);

            Assert.True(result);
        }

        [Fact]
        public async Task When_ValidGetBalances_Expected_Balances()
        {
            var getBalancesDto = new GetBalancesDto()
            {
                PublicKey = _validUser.PublicKey,
                FilterZeroBalances = true,
                PageNumber = 1,
                PageSize = 1
            };

            List<AccountBalances> accountBalances = new()
            {
                new AccountBalances
                {
                    Asset = "XLM",
                    Amount = "10"
                }
            };

            _mockBlockchainService.Setup(x => x.GetBalances(getBalancesDto.PublicKey)).ReturnsAsync(accountBalances);

            var result = await _sut.GetBalances(getBalancesDto);

            Assert.True(result.Balances.Count == accountBalances.Count);
            Assert.Equal(result.Balances[0], accountBalances[0]);
            Assert.True(result.TotalPages == 1);
        }

        [Fact]
        public async Task When_InvalidGetBalances_Expected_ThrowException()
        {
            var getBalancesDto = new GetBalancesDto()
            {
                PublicKey = _validUser.PublicKey,
                FilterZeroBalances = true,
                PageNumber = 1,
                PageSize = 1
            };

            _mockBlockchainService.Setup(x => x.GetBalances(getBalancesDto.PublicKey)).ReturnsAsync((List<AccountBalances>)null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetBalances(getBalancesDto));
        }
    }
}