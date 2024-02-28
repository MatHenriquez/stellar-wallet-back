namespace StellarWallet.Domain.Entities
{
    public class User(string name, string lastName, string email, string password, string publicKey, string secretKey)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public string LastName { get; set; } = lastName;
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
        public string PublicKey { get; set; } = publicKey;
        public string SecretKey { get; set; } = secretKey;
    }
}
