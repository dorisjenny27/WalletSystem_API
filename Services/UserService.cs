using Microsoft.AspNetCore.Identity;
using Services;
using System.Security.Claims;
using WalletSystem.Helpers;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;

namespace WalletSystem.Services
{
    public class UserService(UserManager<User> userManager, IRepository repository, IWalletService walletService) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IRepository _repository = repository;
        private readonly IWalletService _walletService = walletService;

        public async Task<UserProfileDTO> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.First();
            
            if (user == null)
            {
                return null;
            }

            // Map user entity to DTO
            var userProfile = new UserProfileDTO
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                UserType = userRole
                                         
            };

            return userProfile;
        }

        public async Task<bool> PromoteOrDemoteUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("admin"))
            {
                return false;
            }

            if (roleName.ToLower() == "admin")
            {
                if (userRoles.Contains("noob") || userRoles.Contains("elite"))
                {
                    return false;
                }

            }

            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!removeResult.Succeeded)
            {
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, roleName.ToLower());

            return result.Succeeded;
        }


        public async Task<FundWalletResponseDTO> FundUserWallet(string id, FundWalletDTO model)
        {
            var response = new FundWalletResponseDTO();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.Message = $"No record found for user with id: {id}";
                    return response;
                }

                // Check if currency is valid
                if (!UtilityMethods.IsValidCurrencyCode(model.Currency))
                {
                    response.Message = "Invalid currency code!";
                    return response;
                }


                var userRoles = await _userManager.GetRolesAsync(user);
                var currencyExchangeRates = _walletService.LatestConversions;
                var totalBalance = await _repository.GetTotalBalanceByUserIdAsync(user.Id);

                if (userRoles.Contains("noob"))
                {
                    var mainCurrency = user.MainCurrency;

                    var wallet = await _repository.GetByCurrencyAsync(user.Id, mainCurrency);

                    // Check if the transaction currency is different from the user's main currency
                    if (model.Currency != mainCurrency)
                    {
                        // Get the latest conversion rates


                        var convertedAmount = _walletService.ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);

                        model.Amount = convertedAmount;
                    }


                    wallet.AccountBalance += model.Amount;

                    // Update wallet balance
                    var updatedWallet = await _repository.UpdateAsync(wallet);

                    if (!updatedWallet)
                    {
                        response.Message = "An error occurred while updating the wallet";
                        return response;
                    }

                    // Update user's total balance
                    totalBalance.Balance += model.Amount;
                    await _userManager.UpdateAsync(user);

                    response.Message = $"Wallet funded with {mainCurrency} {model.Amount} successfully for user with id: {id}";
                    response.IsSuccess = true;
                }

                else if (userRoles.Contains("elite"))
                {

                    //var wallet = await _repositoryService.GetByCurrencyAsync(model.Currency);
                    var wallet = await _repository.GetByCurrencyAsync(user.Id, model.Currency);

                    if (wallet == null)
                    {

                        var newWallet = new Wallet { UserId = user.Id, CurrencyCode = model.Currency, AccountBalance = model.Amount };
                        await _repository.AddAsync(newWallet);

                        var convertedAmount = _walletService.ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);


                        totalBalance.Balance += convertedAmount; // Update total balance

                        await _userManager.UpdateAsync(user);

                        response.Message = $"New Wallet funded with {model.Currency} {model.Amount} successfully for user with id: {id}";
                        response.IsSuccess = true;
                    }
                    else
                    {
                        wallet.AccountBalance += model.Amount;
                        var walletBalance = await _repository.UpdateAsync(wallet);

                        if (!walletBalance)
                        {
                            response.Message = "An error occurred while updating the wallet";
                            return response;
                        }

                        totalBalance.Balance += model.Amount; // Update total balance
                        await _userManager.UpdateAsync(user);

                        response.Message = $"Wallet funded with {model.Currency} {model.Amount} successfully for user with id: {id}";
                        response.IsSuccess = true;
                    }


                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }


    }
}
