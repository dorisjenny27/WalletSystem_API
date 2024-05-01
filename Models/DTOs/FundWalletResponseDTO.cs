using System.Net.Sockets;

namespace WalletSystem.Models.DTOs
{
    public class FundWalletResponseDTO
    {
        public string? UserId {  get; set; } 
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? TotalBalance { get; set; }
    }
}
