using Microsoft.EntityFrameworkCore;
using Wex.API.Entities;

namespace Wex.API.Repositories
{
    public class MoneyManagementRepository : IMoneyManagementRepository
    {
        MoneyManagementContext _context;

        public MoneyManagementRepository(MoneyManagementContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionAsync(Guid identifier)
        {
            return await _context.Transactions
                                 .Include(t => t.Card)
                                 .AsNoTracking()
                                 .Select(t => new Transaction
                                 {
                                     Amount = t.Amount,
                                     CardId = t.CardId,
                                     CardIdentifier = t.Card.Identifier,
                                     Date = t.Date,
                                     Identifier = t.Identifier,
                                     Description = t.Description,
                                     Id = t.Id
                                 })
                                 .SingleOrDefaultAsync(t => t.Identifier == identifier);
        }

        public async Task<Card?> GetCardAsync(long cardId)
        {
            return await _context.Cards
                                 .AsNoTracking()
                                 .SingleOrDefaultAsync(c => c.Id == cardId);
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Card> AddCardAsync(Card card)
        {
            _context.Cards.Add(card);

            await _context.SaveChangesAsync();

            return card;
        }
    }
}
