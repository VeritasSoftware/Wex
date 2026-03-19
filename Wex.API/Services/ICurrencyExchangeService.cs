using Wex.API.Models;

namespace Wex.API.Services
{
    public interface ICurrencyExchangeService
    {
        Task<ExchangeRateModel?> ConvertCurrencyAsync(DateOnly transactionDate, string? toCountry = null);
    }
}