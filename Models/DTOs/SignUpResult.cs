using System.ComponentModel.DataAnnotations;

namespace WalletSystem.Models.DTOs
{
    public class SignUpResult
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
    }
}
