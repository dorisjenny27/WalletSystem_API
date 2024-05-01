using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WalletSystem.Helpers;
using WalletSystem.Models.DTOs;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;

namespace Services
{
    public class WalletService : IWalletService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly IHttpService _httpService;
        private readonly string? _apikey;
        public Dictionary<string, decimal> LatestConversions { get; private set; }
        public WalletService(UserManager<User> userManager, IAuthService authService, IRepository repository,
            IConfiguration config, IHttpService httpService)
        {
            _userManager = userManager;
            _authService = authService;
            _repository = repository;
            _httpService = httpService;
            _apikey = config["CurrencyApiSettings:ApiKey"];
            LatestConversions = this.GetLatestExchangeRatesAsync().Result;
        }

        public async Task<FundWalletResponseDTO> FundWallet(string id, FundWalletDTO model, ClaimsPrincipal loggedInUser)
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

                var vResult = await _authService.ValidateLoggedInUser(loggedInUser, user.Id);
                if (vResult["Code"] == "400")
                {
                    response.Message = vResult["Message"];
                    return response;
                }

                // Check if currency is valid
                if (!UtilityMethods.IsValidCurrencyCode(model.Currency))
                {
                    response.Message = "Invalid currency code!";
                    return response;
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var currencyExchangeRates = LatestConversions;
              
                var totalBalance = await _repository.GetTotalBalanceByUserIdAsync(user.Id);

                if (userRoles.Contains("noob"))
                {
                    var mainCurrency = user.MainCurrency;

                    var wallet = await _repository.GetByCurrencyAsync(user.Id, mainCurrency);


                    // Check if the transaction currency is different from the user's main currency
                    if (model.Currency != mainCurrency)
                    {
                        // Get the latest conversion rates


                        var convertedAmount = ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);

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

                    var wallet = await _repository.GetByCurrencyAsync(user.Id, model.Currency);

                    if (wallet == null)
                    {

                        var newWallet = new Wallet { UserId = user.Id, CurrencyCode = model.Currency, AccountBalance = model.Amount };
                        await _repository.AddAsync(newWallet);

                        var convertedAmount = ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);


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


        public async Task<WithdrawFundResponseDTO> MakeWithdrawal(string id, WithdrawFundDTO model, ClaimsPrincipal loggedInUser)
        {
            var response = new WithdrawFundResponseDTO();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.Message = $"No record found for user with id: {id}";
                    return response;
                }

                var vResult = await _authService.ValidateLoggedInUser(loggedInUser, user.Id);
                if (vResult["Code"] == "400")
                {
                    response.Message = vResult["Message"];
                    return response;
                }

                // Check if currency is valid
                if (!UtilityMethods.IsValidCurrencyCode(model.Currency))
                {
                    response.Message = "Invalid currency code!";
                    return response;
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var currencyExchangeRates = LatestConversions;
                var totalBalance = await _repository.GetTotalBalanceByUserIdAsync(user.Id);

                if (userRoles.Contains("noob"))
                {
                    var mainCurrency = user.MainCurrency;
                    var wallet = await _repository.GetByCurrencyAsync(user.Id, mainCurrency);

                    if (model.Currency != mainCurrency)
                    {
                        // Get the latest conversion rates

                        var convertedAmount = ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);

                        model.Amount = convertedAmount;
                    }

                    wallet.AccountBalance -= model.Amount;
                    var walletBalance = await _repository.UpdateAsync(wallet);

                    if (!walletBalance)
                    {
                        response.Message = "An error occurred while updating the wallet";
                        return response;
                    }

                    totalBalance.Balance -= model.Amount; // Update total balance
                    await _userManager.UpdateAsync(user);

                    response.Message = $"{mainCurrency} {model.Amount} withdrawn from wallet  successfully for user with id: {id}";
                    response.IsSuccess = true;
                }
                else if (userRoles.Contains("elite"))
                {
                    var mainCurrency = user.MainCurrency;

                    var mainCurrencyWallet = await _repository.GetByCurrencyAsync(user.Id, mainCurrency); ;
                    var wallet = await _repository.GetByCurrencyAsync(user.Id, model.Currency);

                    if (totalBalance.Balance >= model.Amount)
                    {
                        // Check if wallet exists for the specified currency
                        if (wallet == null)
                        {
                            // Convert the withdrawal amount to the main currency
                            var convertedAmount = ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);

                            // Reduce main currency wallet balance
                            mainCurrencyWallet.AccountBalance -= convertedAmount;

                            // Update main currency wallet balance
                            var walletBalance = await _repository.UpdateAsync(mainCurrencyWallet);

                            // Check if the wallet update was successful
                            if (!walletBalance)
                            {
                                response.Message = "An error occurred while updating the wallet";
                                return response;
                            }
                        }
                        else
                        {
                            // If wallet exists, directly reduce the balance
                            wallet.AccountBalance -= model.Amount;

                            // Update wallet balance
                            var walletBalance = await _repository.UpdateAsync(wallet);

                            // Check if the wallet update was successful
                            if (!walletBalance)
                            {
                                response.Message = "An error occurred while updating the wallet";
                                return response;
                            }
                        }

                        var convertedTotalAmount = ConvertCurrency(model.Amount, model.Currency, user.MainCurrency, currencyExchangeRates);


                        // Reduce total balance of the user
                        totalBalance.Balance -= convertedTotalAmount; // Update total balance

                        // Update user's total balance
                        await _userManager.UpdateAsync(user);

                        // Set response message
                        response.Message = $"{(wallet == null ? user.MainCurrency : model.Currency)} {model.Amount} withdrawn from wallet successfully for user with id: {id}";
                        response.IsSuccess = true;
                    }

                    else
                    {
                        response.Message = "Insufficient funds in wallet";
                        return response;
                    }



                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }


        private async Task<Dictionary<string, decimal>> GetLatestExchangeRatesAsync()
        {

            Dictionary<string, string> conversions = new Dictionary<string, string>();
            conversions.Add("apikey", _apikey);

            var request = new ApiRequest
            {
                Endpoint = "latest",
                ApiType = "GET",
                QueryParams = conversions
            };

            var response = await _httpService.MakeRequestAsync<ConversionResponse>(request);

            var currencyExchangeRates = new Dictionary<string, decimal>();

            if (response.IsSuccess)
            {
                var props = (response.Data.Data.GetType().GetProperties()).ToList();
                foreach (var kvp in props)
                {
                    var currencyCode = (Attr)kvp.GetValue(response.Data.Data);
                    string rateString = currencyCode.value;

                    decimal roundedRate = Math.Round(decimal.Parse(rateString, System.Globalization.NumberStyles.Float), 2);

                    currencyExchangeRates.Add(kvp.Name, roundedRate);

                }
            }

            return currencyExchangeRates;
        }

        public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, Dictionary<string, decimal> currencyExchangeRates)
        {
            // Extract conversion rates into a dictionary
            // Dictionary<string, decimal> conversionRates = GetLatestConversionsAsync();

            // Check if conversion rates contain both currencies
            if (currencyExchangeRates.ContainsKey(fromCurrency) && currencyExchangeRates.ContainsKey(toCurrency))
            {
                // Get the conversion rate for the 'fromCurrency' to 'toCurrency'
                decimal fromRate = currencyExchangeRates[fromCurrency];
                decimal toRate = currencyExchangeRates[toCurrency];

                // Perform the conversion
                decimal convertedAmount = Math.Round((decimal)amount * (toRate / fromRate), 2);

                return convertedAmount;
            }
            else
            {
                throw new ArgumentException("Conversion rates for specified currencies are not available.");
            }
        }





    }
}
