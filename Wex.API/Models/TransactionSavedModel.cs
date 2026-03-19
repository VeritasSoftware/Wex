namespace Wex.API.Models
{
    public class TransactionSavedModel
    {
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Guid Identifier { get; set; }
    }
}
