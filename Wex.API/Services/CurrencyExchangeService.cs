using System.Text.Json;
using Wex.API.Models;

namespace Wex.API.Services
{
    public class CurrencyExchangeService
    {
        private readonly IHttpService _http;
        private readonly IConfiguration _configuration;


        public CurrencyExchangeService(IHttpService http, IConfiguration configuration) 
        {
            _http = http;
            _configuration = configuration;
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, DateOnly transactionDate, string? toCountry = null)
        {
            if (toCountry == null)
            {
                toCountry = _configuration["DefaultCurrencyCountry"] ?? "AUD";
            }

            var baseUrl = _configuration["CurrencyExchangeAPIBaseUrl"];

            var endpoint = _configuration["CurrencyExchangeAPIEndpoint"];

            var url = $@"{baseUrl}{endpoint}?fields=country_currency_desc,exchange_rate,record_date&filter=country:in:({toCountry.ToLower()}),record_date:lte:{transactionDate}";

            var response = await _http.Client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var actualContent = JsonSerializer.Deserialize<CurrencyExchangeResponse>(content);

            var exchangeRate = actualContent?.Data.FirstOrDefault()?.ExchangeRate;

            var result = amount * decimal.Parse(exchangeRate ?? "1");

            return result;
        }
    }
}
