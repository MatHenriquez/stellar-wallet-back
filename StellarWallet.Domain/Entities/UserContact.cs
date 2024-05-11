using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StellarWallet.Domain.Entities
{
    public class UserContact(string alias, int userId, int blockchainAccountId)
    {
        [Key]
        public int Id { get; private set; }

        [Required(ErrorMessage = "Alias is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Alias must have a maximum of 50 characters and a minimum of 3")]
        public string Alias { get; set; } = alias;

        [Required(ErrorMessage = "User id is required")]
        public int UserId { get; set; } = userId;

        [Required(ErrorMessage = "BlockchainAccount id is required")]
        public int BlockchainAccountId { get; set; } = blockchainAccountId;

        [JsonIgnore]
        public User? User { get; set; } = null;

        [JsonIgnore]
        public BlockchainAccount? BlockchainAccount { get; set; } = null;
    }
}
