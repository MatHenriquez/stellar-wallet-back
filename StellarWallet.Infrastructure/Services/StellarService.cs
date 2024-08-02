using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Structs;
using StellarWallet.Infrastructure.Utilities;

namespace StellarWallet.Infrastructure.Stellar
{
    public class StellarService : IBlockchainService
    {
        private readonly string EnvironmentName;
        private readonly string horizonUrl;
        private readonly Network network;
        private readonly Server server;

        public StellarService()
        {
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";
            horizonUrl = AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.HorizonUrl, EnvironmentName);
            network = new Network(AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.Passphrase, EnvironmentName));
            server = new Server(AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.HorizonUrl, EnvironmentName));
        }

        public BlockchainAccount CreateAccount(int userId)
        {
            KeyPair keyPair = KeyPair.Random();
            return new BlockchainAccount(PublicKey: keyPair.AccountId, SecretKey: keyPair.SecretSeed, userId);
        }

        public AccountKeyPair CreateKeyPair()
        {
            KeyPair keyPair = KeyPair.Random();

            var accountKeyPair = new AccountKeyPair
            {
                PublicKey = keyPair.AccountId,
                SecretKey = keyPair.SecretSeed
            };

            return accountKeyPair;
        }

        public async Task<bool> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount)
        {
            KeyPair sourceKeypair = KeyPair.FromSecretSeed(sourceSecretKey);

            AccountResponse sourceAccountResponse;
            try
            {
                sourceAccountResponse = await server.Accounts.Account(sourceKeypair.AccountId);

                KeyPair destinationKeyPair = KeyPair.FromAccountId(destinationPublicKey);
                Account sourceAccount = new Account(sourceKeypair.AccountId, sourceAccountResponse.SequenceNumber);

                PaymentOperation paymentOperation = new PaymentOperation.Builder(destinationKeyPair, new AssetTypeNative(), amount).SetSourceAccount(sourceAccount.KeyPair).Build();

                Transaction transaction = new TransactionBuilder(sourceAccount)
                   .AddOperation(paymentOperation)
                   .Build();

                transaction.Sign(sourceKeypair, network);

                SubmitTransactionResponse response = await server.SubmitTransaction(transaction);
                return response.IsSuccess();
            }
            catch (Exception e)
            {
                throw new Exception("Stellar Error " + e.Message);
            }
        }

        public async Task<BlockchainPayment[]> GetPayments(string accountId)
        {
            var payments = new List<BlockchainPayment>();

            var server = new Server("https://horizon-testnet.stellar.org");
            var paymentsRequestBuilder = server.Payments.ForAccount(accountId);

            try
            {
                var page = await paymentsRequestBuilder.Execute();

                while (true)
                {
                    foreach (var record in page.Records.OfType<PaymentOperationResponse>())
                    {
                        payments.Add(new BlockchainPayment
                        {
                            Id = record.TransactionHash,
                            From = record.From,
                            To = record.To,
                            Amount = record.Amount,
                            Asset = record.Asset is AssetTypeNative ? "native" : ((AssetTypeCreditAlphaNum)record.Asset).Code,
                            CreatedAt = record.CreatedAt.ToString(),
                            WasSuccessful = record.TransactionSuccessful
                        });
                    }

                    if (page.Records.Count == 0)
                    {
                        break;
                    }

                    page = await paymentsRequestBuilder.Cursor(page.Records.Last().PagingToken).Execute();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Stellar Error " + e.Message);
            }

            return [.. payments];
        }

        public async Task<bool> GetTestFunds(string accountId)
        {
            try
            {
                var friendBotRequest = new HttpRequestMessage(HttpMethod.Get, $"{horizonUrl}/friendbot?addr={accountId}");
                var response = await new HttpClient().SendAsync(friendBotRequest);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }

        }

        public async Task<AccountBalances[]> GetBalances(string accountId)
        {
            var balances = new List<AccountBalances>();

            var account = await server.Accounts.Account(accountId);

            foreach (var balance in account.Balances)
            {
                balances.Add(new AccountBalances
                {
                    Asset = balance.Asset is AssetTypeNative ? "native" : ((AssetTypeCreditAlphaNum)balance.Asset).Code,
                    Amount = balance.BalanceString,
                });
            }

            return [.. balances];
        }
    }
}
