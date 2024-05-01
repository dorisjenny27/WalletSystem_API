using System.ComponentModel.DataAnnotations.Schema;

namespace WalletSystem.Models.Entity
{
    [Table("Accounts")]
    public class Wallet
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();

        public string? CurrencyCode { get; set; }

        public decimal? AccountBalance { get; set; }

        public string UserId { get; set; } = "";
        public User User { get; set; }
    }
}
