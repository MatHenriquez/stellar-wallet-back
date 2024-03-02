using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Infrastructure.Stellar
{
    public class Stellar : IBlockchainService
    {
        private readonly Network network = new("Test SDF Network ; September 2015");
        private readonly Server server = new Server("https://horizon-testnet.stellar.org");

        public StellarAccount CreateAccount()
        {
            KeyPair keyPair = KeyPair.Random();
            return new StellarAccount(PublicKey: keyPair.AccountId, SecretKey: keyPair.SecretSeed);
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
    }
}
