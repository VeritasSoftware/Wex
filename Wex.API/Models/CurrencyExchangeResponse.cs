using System.Text.Json.Serialization;

namespace Wex.API.Models
{    
    public class CurrencyExchangeResponse
    {
        [JsonPropertyName("data")]
        public List<Data> Data { get; set; } = new List<Data>();
    }

    public class Data
    {
        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDesc { get; set; } = null!;

        [JsonPropertyName("exchange_rate")]
        public string ExchangeRate { get; set; } = null!;

        [JsonPropertyName("record_date")]
        public DateOnly RecordDate { get; set; }
    }
}
