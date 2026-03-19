using System.Globalization;
using Wex.API.Models;
using Wex.API.Repositories;

namespace Wex.API.Services
{
    public class MoneyManagementService : IMoneyManagementService
    {
        private readonly IMoneyManagementRepository _repository;
        private readonly ICurrencyExchangeService _currencyExchangeService;
        private readonly ILogger<MoneyManagementService>? _logger;

        public MoneyManagementService(IMoneyManagementRepository repository, ICurrencyExchangeService currencyExchangeService,
                                        ILogger<MoneyManagementService>? logger = null)
        {
            _repository = repository;
            _currencyExchangeService = currencyExchangeService;
            _logger = logger;
        }

        public async Task<TransactionModel?> GetTransactionAsync(string identifier, string? country = null)
        {
            var identifierGuid = Guid.Parse(identifier);

            _logger?.LogInformation($"Fetching transaction: {identifier} from database.");

            var transactionDb = await _repository.GetTransactionAsync(identifierGuid);

            if (transactionDb == null)
            {
                _logger?.LogError($"Transaction: {identifier} not found in database.");

                return null;
            }

            _logger?.LogInformation($"Calling currency exchange api to determine exchange rate.");

            var exchangeRate = await _currencyExchangeService.ConvertCurrencyAsync(transactionDb.Date, country);

            if (exchangeRate == null)
            {
                _logger?.LogError($"Exchange rate for country: {country} not found.");
                return null;
            }

            var convertedAmount = transactionDb.Amount * decimal.Parse(exchangeRate.ExchangeRate ?? "1");

            return new TransactionModel
            {
                Amount = convertedAmount,
                CurrencyCode = exchangeRate.CurrencyCode,
                Date = transactionDb.Date.ToString(CultureInfo.CurrentCulture),
                Description = transactionDb.Description,
                Identifier = transactionDb.Identifier,
            };
        }
    }
}
