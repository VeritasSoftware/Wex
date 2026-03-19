using Wex.API.Models;

namespace Wex.API.Services
{
    public interface IMoneyManagementService
    {
        Task<TransactionModel?> GetTransactionAsync(string identifier, string? country = null);
        Task<TransactionSavedModel> AddTransactionAsync(TransactionCreateModel transaction);
    }
}