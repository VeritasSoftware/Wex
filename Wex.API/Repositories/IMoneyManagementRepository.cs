using Wex.API.Entities;

namespace Wex.API.Repositories
{
    public interface IMoneyManagementRepository
    {
        Task<Card> AddCardAsync(Card card);
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<Card?> GetCardAsync(long cardId);
        Task<Transaction?> GetTransactionAsync(Guid identifier);
    }
}