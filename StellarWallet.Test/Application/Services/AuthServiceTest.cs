using Moq;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.UnitTest.Application.Services
{
    public class AuthServiceTest
    {
        [Fact]
        public async Task When_ValidLoginDto_Expected_ReturnLoggedDto()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            var mockJwtService = new Mock<IJwtService>();
            var mockEncryptionService = new Mock<IEncryptionService>();

            var sut = new AuthService(mockUserRepository.Object, mockJwtService.Object, mockEncryptionService.Object);

            var loggedDto = new LoggedDto(true, "token", "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA");

            var loginDto = new LoginDto("john.doe@mail.com", "MyPassword123.");

            mockUserRepository.Setup(x => x.GetBy("Email", loginDto.Email)).ReturnsAsync(new User(
                "John",
                "Doe",
                "john.doe@mail.com",
                "EncryptedPassword",
                "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA",
                "SC6KTRKOT33RRH2KXK2BMGJWJ7TE5NQGRE5NIWTBGNMSPLKGQ2C63KDB"
            ));

            mockEncryptionService.Setup(x => x.Verify(loginDto.Password, "EncryptedPassword")).Returns(true);

            mockJwtService.Setup(x => x.CreateToken("john.doe@mail.com", "user")).Returns("token");

            var result = await sut.Login(loginDto);

            Assert.True(result.Success);
            Assert.Equal(loggedDto.Token, result.Token);
            Assert.Equal(loggedDto.PublicKey, result.PublicKey);
        }
    }
}
