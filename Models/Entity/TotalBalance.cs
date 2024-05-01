namespace WalletSystem.Models.Entity
{
    public class TotalBalance
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public decimal? Balance { get; set; }
        public User User { get; set; }
        public string UserId { get; set; } = "";
    }
}
