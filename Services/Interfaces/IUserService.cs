using WalletSystem.Models.DTOs;

namespace WalletSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> PromoteOrDemoteUser(string userId, string roleName);

        Task<FundWalletResponseDTO> FundUserWallet(string id, FundWalletDTO model);
    }
}
