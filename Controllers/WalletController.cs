using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WalletSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly IHttpService _httpService;
        private readonly IConfiguration _config;
        private readonly IWalletService _walletService;

       // public Dictionary<string, double> LatestConversions => throw new NotImplementedException();

        public WalletController(ILogger<WalletController> logger, IConfiguration config, IWalletService walletService,
         IHttpService httpService)
        {
            _logger = logger;
            _config = config;
            _walletService = walletService;
            _httpService = httpService;

        }

        [HttpGet("get-latest-conversions")]
        public async Task<IActionResult> GetLatest()
        {
            var result = _walletService.LatestConversions;
            return Ok(result);
        }

        [HttpGet("get-single/{code}")]
        public async Task<IActionResult> GetLatesot(string code)
        {
            var result = _walletService.LatestConversions.FirstOrDefault(x => x.Key == code);
            return Ok(result);
        }

    //{
    //    [Authorize(Roles = "Elite")] // Only allow Elite users to access these endpoints
    //    [Route("api/[controller]")]
    //    [ApiController]
    //    public class WalletController : ControllerBase
    //    {
    //        private readonly IWalletService _walletService;

    //        public WalletController(IWalletService walletService)
    //        {
    //            _walletService = walletService;
    //        }

    //        [Authorize(Roles = "Elite")] 
    //        [HttpPost("create")]
    //        public IActionResult CreateWallet(CreateWalletRequest request)
    //        {
    //            var success = _walletService.CreateOrUpdateWallet(request);

    //            if (success)
    //            {
    //                return Ok("Wallet created/updated successfully.");
    //            }
    //            else
    //            {
    //                return BadRequest("Failed to create/update wallet.");
    //            }
    //        }


        }
}
