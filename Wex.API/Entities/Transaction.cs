namespace Wex.API.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public Guid Identifier { get; set; }
        public decimal? Amount { get; set; }

        // Foreign key
        public long CardId { get; set; }
        public virtual Card Card { get; set; } = null!;

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier, Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Transaction other)
            {
                return this.GetHashCode() == other.GetHashCode();
            }
            return false;
        }
    }
}
