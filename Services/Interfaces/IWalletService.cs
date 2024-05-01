using System.Security.Claims;
using WalletSystem.Models.DTOs;

namespace WalletSystem.Services.Interfaces
{
    public interface IWalletService
    {
        //Task<Dictionary<string, double>> GetLatestConversions();
        Dictionary<string, decimal> LatestConversions { get; }
        Task<FundWalletResponseDTO> FundWallet(string id, FundWalletDTO model, ClaimsPrincipal loggedInUser);
        Task<WithdrawFundResponseDTO> MakeWithdrawal(string id, WithdrawFundDTO model, ClaimsPrincipal loggedInUser);
        decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, Dictionary<string, decimal> currencyExchangeRates);
    } 
}
