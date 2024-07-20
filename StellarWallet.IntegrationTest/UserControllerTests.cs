using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using System.Net.Http.Json;

namespace StellarWallet.IntegrationTest
{
    public class UserControllerTests
    {
        public UserControllerTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
        }

        [Fact]
        public async Task When_ValidUserCreationDto_Expected_ReturnLoggedDto()
        {
            var application = new StellarWalletWebApplicationFactory();

            UserCreationDto request = new("John", "Doe", "my.mail2@test.com", "MyPassword123.", null, null);

            var client = application.CreateClient();

            var response = await client.PostAsJsonAsync("/User", request);

            response.EnsureSuccessStatusCode();
            var createUserResponse = await response.Content.ReadFromJsonAsync<LoggedDto>();

            Assert.True(createUserResponse?.Success);
            Assert.Null(createUserResponse?.Token);
            Assert.NotNull(createUserResponse?.PublicKey);
        }
    }
}
