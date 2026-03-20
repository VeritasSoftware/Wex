using System.Globalization;
using Wex.API.Entities;
using Wex.API.Models;
using Wex.API.Repositories;

namespace Wex.API.Services
{
    public class MoneyManagementService : IMoneyManagementService
    {
        private readonly IMoneyManagementRepository _repository;
        private readonly ICurrencyExchangeService _currencyExchangeService;
        private readonly IMapperService _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MoneyManagementService>? _logger;

        public MoneyManagementService(IMoneyManagementRepository repository, ICurrencyExchangeService currencyExchangeService,
                                      IMapperService mapper, IConfiguration configuration, ILogger<MoneyManagementService>? logger = null)
        {
            _repository = repository;
            _currencyExchangeService = currencyExchangeService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CardBalanceModel?> GetCardBalanceAsync(string identifier, string? country = null)
        {
            var identifierGuid = Guid.Parse(identifier);

            _logger?.LogInformation($"Fetching card: {identifier} balance from database.");

            var balanceDb = await _repository.GetCardBalanceAsync(identifierGuid);

            if (balanceDb == null)
            {
                _logger?.LogError($"Card: {identifier} not found in database.");

                return null;
            }

            if (country == null || country == _configuration["DefaultCurrencyCountry"])
            {
                return new CardBalanceModel
                {
                    Balance = balanceDb.Balance,                    
                    CurrencyCode = "Dollar",
                    Identifier = balanceDb.Identifier.ToString(),
                };
            }

            _logger?.LogInformation($"Calling currency exchange api to determine exchange rate.");

            var exchangeRate = await _currencyExchangeService.ConvertCurrencyAsync(DateOnly.FromDateTime(DateTime.Now), country);

            if (exchangeRate == null)
            {
                _logger?.LogError($"Exchange rate for country: {country} not found.");
                return null;
            }

            var convertedAmount = balanceDb.Balance * decimal.Parse(exchangeRate.ExchangeRate ?? "1");

            return new CardBalanceModel
            {
                Balance = convertedAmount,
                CurrencyCode = exchangeRate.CurrencyCode,
                Identifier = balanceDb.Identifier.ToString()
            };
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

            if (country == null || country == _configuration["DefaultCurrencyCountry"])
            {
                return new TransactionModel
                {
                    Amount = transactionDb.Amount,
                    CurrencyCode = "Dollar",
                    Date = FromUniversalDateTime(transactionDb.Date),
                    Description = transactionDb.Description,
                    Identifier = transactionDb.Identifier,
                    CardIdentifier = transactionDb.CardIdentifier,
                };
            }

            _logger?.LogInformation($"Calling currency exchange api to determine exchange rate.");

            var exchangeRate = await _currencyExchangeService.ConvertCurrencyAsync(DateOnly.FromDateTime(transactionDb.Date), country);

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
                Date = FromUniversalDateTime(transactionDb.Date),
                Description = transactionDb.Description,
                Identifier = transactionDb.Identifier,
                CardIdentifier = transactionDb.CardIdentifier,
            };
        }

        private static string FromUniversalDateTime(DateTime date)
        {
            // Convert to local time
            DateTime localDateTime = date.ToLocalTime();

            return DateOnly.FromDateTime(localDateTime.Date).ToString(CultureInfo.CurrentCulture);
        }

        public async Task<TransactionSavedModel> AddTransactionAsync(TransactionCreateModel transaction)
        {
            var dbTransaction = _mapper.Map<TransactionCreateModel, Transaction>(transaction);

            var transactionInDb = await _repository.AddTransactionAsync(dbTransaction);

            return _mapper.Map<Transaction, TransactionSavedModel>(transactionInDb);
        }

        public async Task<CardModel> AddCardAsync(CardCreateModel card)
        {
            var dbCard = _mapper.Map<CardCreateModel, Card>(card);

            var cardInDb = await _repository.AddCardAsync(dbCard);

            return _mapper.Map<Card, CardModel>(cardInDb);
        }
    }
}
