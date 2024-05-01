using Newtonsoft.Json;
using WalletSystem.Models.DTOs;
using WalletSystem.Services.Interfaces;

namespace WalletSystem.Services
{
    public class HttpService : IHttpService
    {
        private readonly string _baseUrl = "";
        // private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public HttpService(IConfiguration config, HttpClient httpClient) 
        {
            _baseUrl = config.GetSection("APISettings:BaseUrl").Value;
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<T>> MakeRequestAsync<T>(ApiRequest request)
        {
            // using var client = new HttpClient();

            if (request.QueryParams.Any())
            {
                foreach(var kvp in request.QueryParams)
                {
                    _httpClient.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            HttpResponseMessage? res = null;
            switch (request.ApiType)
            {
                case "GET":
                    res = await _httpClient.GetAsync(new Uri($"{_baseUrl}{request.Endpoint}"));
                    break;
                default:
                    return new ApiResponse<T> { Error = "Api type is not supported" };
                    break;
            }
            if(res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(content);

                return new ApiResponse<T>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            return new ApiResponse<T>();
        }
    }
}
