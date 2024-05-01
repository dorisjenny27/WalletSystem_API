using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;

namespace WalletSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromBody] SignUpDTO model)
        {
            try
            {
                var user = await _authService.CreateUser(model);
                return Ok($"User added with Id: {user.Id}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                var loginResult = await _authService.Login(model.Email, model.Password);
                return Ok(loginResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}