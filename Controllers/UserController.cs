using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletSystem.Models.DTOs;
using WalletSystem.Services.Interfaces;
using WalletSystem.Models.DTOs;

namespace WalletSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AdminController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPut("promote-or-demote-user/{userId}")]
        public async Task<IActionResult> PromoteOrDemoteUser(string userId, [FromBody] ChangeUserRoleDTO model)
        {
            var response = await _userService.PromoteOrDemoteUser(userId, model.NewRole);

            return response ? Ok("User role updated successfully") : BadRequest("An error occurred");
        }

        [HttpPut("fund-user-wallet/{id}")]
        public async Task<IActionResult> FundUserWallet(string id, [FromBody] FundWalletDTO model)
        {
            var response = await _userService.FundUserWallet(id, model);

            return response.IsSuccess ? Ok(response.Message) : BadRequest(response.Message);
        }

    }
}
