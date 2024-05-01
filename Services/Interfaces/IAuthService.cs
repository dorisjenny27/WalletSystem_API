using System.Security.Claims;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;

namespace WalletSystem.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJWT(User user, List<string> roles, List<Claim> claims);

        Task<LoginResult> Login(string email, string password);

        Task<Dictionary<string, string>> ValidateLoggedInUser(ClaimsPrincipal user, string userId);

        Task<User> CreateUser(SignUpDTO model);
    }
}
