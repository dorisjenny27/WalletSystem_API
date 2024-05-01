using Microsoft.AspNetCore.Identity;

namespace WalletSystem.Models.Entity
{
    public class User : IdentityUser
    {
        public string? UserType { get; set; }
        public string? MainCurrency { get; set; }
        public IList<Wallet>? Wallets {  get; set; } //One to one relationship btween User and wallet

        public TotalBalance TotalBalance { get; set; }
    }
}
