using System.ComponentModel.DataAnnotations;

namespace WalletSystem.Models.DTOs
{
    public class WithdrawFundDTO
    {
        public string Currency { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
