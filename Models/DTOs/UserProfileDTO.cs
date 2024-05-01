
namespace WalletSystem.Models.DTOs
{
    public class UserProfileDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }

    }

    public enum UserType
    {
        Noob,
        Elite,
        Admin
    }
}

