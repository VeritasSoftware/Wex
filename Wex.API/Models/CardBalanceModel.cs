namespace Wex.API.Models
{
    public class CardBalanceModel
    {
        public string Identifier { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public string CurrencyCode { get; set; } = "Dollar";
    }
}
