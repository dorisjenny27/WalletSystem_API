using System.ComponentModel.DataAnnotations;

namespace WalletSystem.Models.DTOs
{
    public class UpdateUserDTO
    {
        [RegularExpression(@"234-[0-9]{10}")]
        public string? PhoneNumber { get; set; }
    }
}
