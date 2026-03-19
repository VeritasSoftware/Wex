using System.Text.Json;
using Wex.API.Models;

namespace Wex.API.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly IHttpService _http;
        private readonly IConfiguration _configuration;


        public CurrencyExchangeService(IHttpService http, IConfiguration configuration)
        {
            _http = http;
            _configuration = configuration;
        }

        public async Task<ExchangeRateModel?> ConvertCurrencyAsync(DateOnly transactionDate, string? toCountry = null)
        {
            if (toCountry != null && toCountry == _configuration["DefaultCurrencyCountry"])  
            {
                return null;
            }

            var baseUrl = _configuration["CurrencyExchangeAPIBaseUrl"];

            var endpoint = _configuration["CurrencyExchangeAPIEndpoint"];

            var url = $@"{baseUrl}{endpoint}?fields=country,currency,country_currency_desc,exchange_rate,record_date&filter=country:in:({CapitalizeFirstLetter(toCountry.ToLower())}),record_date:lte:{transactionDate.ToString("yyyy-MM-dd")}&sort=-record_date";

            var response = await _http.Client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var deserialisedResponse = JsonSerializer.Deserialize<CurrencyExchangeResponse>(content);

            var exchangeRate = deserialisedResponse?.Data.FirstOrDefault();

            return exchangeRate;
        }

        public static string CapitalizeFirstLetter(string input)
        {
            // Handle null or empty strings
            if (string.IsNullOrEmpty(input))
                return input;

            // If the first character is already uppercase, return as is
            if (char.IsUpper(input[0]))
                return input;

            // Capitalize first letter and append the rest unchanged
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
