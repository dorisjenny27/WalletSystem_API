using WalletSystem.Models.DTOs;

namespace WalletSystem.Services.Interfaces
{
    public interface IHttpService
    {
        Task<ApiResponse<T>> MakeRequestAsync<T>(ApiRequest request);
    }
}
