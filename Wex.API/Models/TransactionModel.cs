namespace Wex.API.Models
{
    public class TransactionModel
    {
        public string Description { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public Guid Identifier { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyCode { get; set; } = "AUD";

        // Foreign key
        public long CardId { get; set; }
        public virtual CardModel Card { get; set; } = null!;

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier);
        }

        public override bool Equals(object? obj)
        {
            if (obj is TransactionModel other)
            {
                return this.GetHashCode() == other.GetHashCode();
            }
            return false;
        }
    }
}
