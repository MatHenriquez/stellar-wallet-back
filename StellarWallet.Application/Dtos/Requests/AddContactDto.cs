using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests
{
    public class AddContactDto(string alias, int userId)
    {
        [Required(ErrorMessage = "Alias is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Alias must have a maximum of 50 characters and a minimum of 3")]
        public string Alias { get; set; } = alias;

        [Required(ErrorMessage = "User id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User id must be positive")]
        public int UserId { get; set; } = userId;
    }
}
