using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;

namespace WalletSystem.Services
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IRepository _repository;
        public AuthService(IConfiguration config, UserManager<User> userManager, IRepository repository)
        {
            _config = config;
            _userManager = userManager;
            _repository = repository;   
        }

        public string GenerateJWT(User user, List<string> roles, List<Claim> claims)
        {
            var myClaims = new List<Claim>();

            myClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            myClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            myClaims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
            {
                myClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var claim in claims)
            {
                myClaims.Add(new Claim(claim.Type, claim.Value));
            }
            var key = Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: myClaims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        public async Task<LoginResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = new List<Claim>();
                    return new LoginResult
                    {
                        IsSuccess = true,
                        Message = "Login sucessfully!",
                        UserId = user.Id,
                        Roles = roles.ToList(),
                        Token = this.GenerateJWT(user, roles.ToList(), claims)
                    };
                }
            }
            return new LoginResult
            {
                IsSuccess = false,
                Message = "Invalid credential"
            };
        }

        public async Task<Dictionary<string, string>> ValidateLoggedInUser(ClaimsPrincipal user, string userId)
        {
            var loggedInUser = await _userManager.GetUserAsync(user);
            if (loggedInUser == null || loggedInUser.Id != userId)
            {
                return new Dictionary<string, string> {
                    { "Code", "400" },
                    { "Message", "Access denied! Id provided does not match loggedIn user." }
                };
            }

            return new Dictionary<string, string> {
                    { "Code", "200" },
                    { "Message", "ok" }
                };
        }

        public async Task<User> CreateUser(SignUpDTO model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                throw new ArgumentException("Email already exists!");

            // var user = _mapper.Map<UserToAddDTO, User>(model);
            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
//                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors));

            await _userManager.AddToRoleAsync(user, model.UserType.ToLower());


            var newWallet = new Wallet { UserId = user.Id, CurrencyCode = model.CurrencyType, AccountBalance = 0 };
            await _repository.AddAsync(newWallet);

            var totalBalance = new TotalBalance { UserId = user.Id, Balance = 0 };
            await _repository.AddAsync(totalBalance);



            return user;
        }
    }
}
