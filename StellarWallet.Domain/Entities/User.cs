using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Domain.Entities
{
    public class User(string name, string lastName, string email, string password, string publicKey, string secretKey)
    {
        [Required(ErrorMessage = "Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
        public int Id { get; private set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must have a maximum of 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Special characters are not allowed.")]
        public string Name { get; set; } = name;

        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(50, ErrorMessage = "Name must have a maximum of 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Special characters are not allowed.")]
        public string LastName { get; set; } = lastName;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = email;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must have a maximum of 50 characters")]
        public string Password { get; set; } = password;

        [Required(ErrorMessage = "Public key is required")]
        [StringLength(57, ErrorMessage = "Public key must have a maximum of 57 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,57}$", ErrorMessage = "Special characters are not allowed.")]
        public string PublicKey { get; set; } = publicKey;

        [Required(ErrorMessage = "Secret key is required")]
        [StringLength(57, ErrorMessage = "Secret key must have a maximum of 57 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,57}$", ErrorMessage = "Special characters are not allowed.")]
        public string SecretKey { get; set; } = secretKey;
    }
}
