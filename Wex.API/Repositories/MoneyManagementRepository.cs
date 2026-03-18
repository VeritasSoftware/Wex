using Microsoft.EntityFrameworkCore;
using Wex.API.Models;

namespace Wex.API.Repositories
{
    public class MoneyManagementRepository
    {
        MoneyManagementContext _context;

        public MoneyManagementRepository(MoneyManagementContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionAsync(Guid identifier)
        {
            return await _context.Transactions
                                 .AsNoTracking()
                                 .SingleOrDefaultAsync(t => t.Identifier == identifier);
        }

        public async Task<Card?> GetCardAsync(long cardId)
        {
            return await _context.Cards
                                 .AsNoTracking()
                                 .SingleOrDefaultAsync(c => c.Id == cardId);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
        }
    }
}
