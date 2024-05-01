namespace WalletSystem.Models.DTOs
{
    public class WithdrawFundResponseDTO
    {
        public string UserId { get; set; } = "";
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = "";
    }
}
