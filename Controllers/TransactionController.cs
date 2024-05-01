using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletSystem.Models.DTOs;
using WalletSystem.Services.Interfaces;
using WalletSystem.Services;

namespace WalletSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(IWalletService walletService) : ControllerBase
    {

        private readonly IWalletService _walletService = walletService;

        [HttpPut("make-deposit/{id}")]
        [Authorize(Roles = "noob, elite, ")]
        public async Task<IActionResult> MakeDeposit(string id, [FromBody] FundWalletDTO model)
        {
            // Call the service method to fund the wallet
            var response = await _walletService.FundWallet(id, model, User);

            return response.IsSuccess ? Ok(response.Message) : BadRequest(response.Message);
        }

        [HttpPut("make-withdrawal/{id}")]
        public async Task<IActionResult> MakeWithdrawal(string id, [FromBody] WithdrawFundDTO model)
        {
            // Call the service method to fund the wallet
            var response = await _walletService.MakeWithdrawal(id, model, User);

            return response.IsSuccess ? Ok(response.Message) : BadRequest(response.Message);
        }

    }
}
