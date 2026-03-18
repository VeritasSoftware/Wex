namespace Wex.API.Models
{
    public class Card
    {
        public long Id { get; set; }
        public decimal? CreditLimit { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
