using System.ComponentModel.DataAnnotations;
using WalletSystem.Models.DTOs;

namespace WalletSystem.Models.DTOs
{
    public class SignUpDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public string? UserType { get; set; }

        public string CurrencyType { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }

 
}
