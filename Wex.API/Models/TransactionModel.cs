namespace Wex.API.Models
{
    public class TransactionModel
    {
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public Guid Identifier { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyCode { get; set; } = "AUD";

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
