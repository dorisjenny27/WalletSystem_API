namespace WalletSystem.Models.DTOs
{
    public class ApiRequestModel
    {
        public string ApiType { get; set; }
        public string Url { get; set; }

        public object Data { get; set; }
        public string AccessToken { get; set; }

        public Dictionary<string, string> QueryParams { get; set; }

        public ApiRequestModel()
        {
            QueryParams = new Dictionary<string, string>();
        }
    }
}
