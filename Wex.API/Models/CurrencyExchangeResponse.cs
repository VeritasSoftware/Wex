using System.Text.Json.Serialization;

namespace Wex.API.Models
{    
    public class CurrencyExchangeResponse
    {
        [JsonPropertyName("data")]
        public List<ExchangeRateModel> Data { get; set; } = new List<ExchangeRateModel>();
    }

    public class ExchangeRateModel
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; set; } = null!;

        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDesc { get; set; } = null!;

        [JsonPropertyName("exchange_rate")]
        public string ExchangeRate { get; set; } = null!;

        [JsonPropertyName("record_date")]
        public DateOnly RecordDate { get; set; }
    }
}
