namespace Wex.API.Models
{
    public class TransactionCreateModel
    {
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public long CardId { get; set; }
    }
}
