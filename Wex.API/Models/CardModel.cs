namespace Wex.API.Models
{
    public class CardModel
    {
        public long Id { get; set; }
        public decimal? CreditLimit { get; set; }
        public Guid Identifier { get; set; }
    }
}
