using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;
using StellarWallet.Infrastructure.Utilities;

namespace StellarWallet.Infrastructure.Services
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

        public async Task<Result<bool, DomainError>> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount, string assetIssuer, string assetCode, string memo)
        {
            var sourceKeypair = KeyPair.FromSecretSeed(sourceSecretKey);

            AccountResponse sourceAccountResponse;

            sourceAccountResponse = await server.Accounts.Account(sourceKeypair.AccountId);

            var destinationKeyPair = KeyPair.FromAccountId(destinationPublicKey);

            if (sourceAccountResponse is null || destinationKeyPair is null)
            {
                return Result<bool, DomainError>.Failure(DomainError.ExternalServiceError("Invalid account"));
            }

            Account sourceAccount = new Account(sourceKeypair.AccountId, sourceAccountResponse.SequenceNumber);

            var asset = assetIssuer == "native" ? (Asset)new AssetTypeNative() : Asset.CreateNonNativeAsset(assetCode, assetIssuer);

            PaymentOperation paymentOperation = new PaymentOperation.Builder(destinationKeyPair, asset, amount).SetSourceAccount(sourceAccount.KeyPair).Build();

            Transaction transaction = new TransactionBuilder(sourceAccount)
               .AddOperation(paymentOperation)
               .AddMemo(new MemoText(memo))
               .Build();

            transaction.Sign(sourceKeypair, network);

            SubmitTransactionResponse response = await server.SubmitTransaction(transaction);

            if (response.IsSuccess())
            {
                return Result<bool, DomainError>.Success(response.IsSuccess());
            }

            return Result<bool, DomainError>.Failure(DomainError.ExternalServiceError("Transaction failed"));
        }

        public async Task<Result<BlockchainPayment[], DomainError>> GetPayments(string accountId)
        {
            var payments = new List<BlockchainPayment>();

            try
            {
                var server = new Server(horizonUrl);
                var paymentsRequestBuilder = server.Payments.ForAccount(accountId);
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
                return Result<BlockchainPayment[], DomainError>.Failure(DomainError.ExternalServiceError("Stellar Error " + e.Message));
            }

            return Result<BlockchainPayment[], DomainError>.Success([.. payments]);
        }

        public async Task<Result<bool, DomainError>> GetTestFunds(string accountId)
        {
            try
            {
                var friendBotRequest = new HttpRequestMessage(HttpMethod.Get, $"{horizonUrl}/friendbot?addr={accountId}");
                var response = await new HttpClient().SendAsync(friendBotRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<bool, DomainError>.Failure(DomainError.ExternalServiceError("Test funds failed"));
                }

                return Result<bool, DomainError>.Success(true);
            }
            catch (Exception e)
            {
                return Result<bool, DomainError>.Failure(DomainError.ExternalServiceError("Stellar Error " + e.Message));
            }
        }

        public async Task<Result<List<AccountBalances>, DomainError>> GetBalances(string accountId)
        {
            try
            {
                var balances = new List<AccountBalances>();

                var account = await server.Accounts.Account(accountId);

                foreach (var balance in account.Balances)
                {
                    balances.Add(new AccountBalances
                    {
                        Asset = balance.Asset is AssetTypeNative ? "native" : ((AssetTypeCreditAlphaNum)balance.Asset).Code,
                        Amount = balance.BalanceString,
                        Issuer = balance.Asset is AssetTypeNative ? "native" : ((AssetTypeCreditAlphaNum)balance.Asset).Issuer
                    });
                }

                return Result<List<AccountBalances>, DomainError>.Success(balances);
            }
            catch (Exception e)
            {
                return Result<List<AccountBalances>, DomainError>.Failure(DomainError.ExternalServiceError("Stellar Error " + e.Message));
            }
        }
    }
}
