using Microsoft.AspNetCore.Mvc.Testing;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Wex.API;
using Wex.API.Models;

namespace Wex.Tests.API
{
    public class APITests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public APITests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData("128fb236-9666-4e8a-945e-7f55b800bca4", "Canada", 958.9845, "Dollar")]
        [InlineData("128fb236-9666-4e8a-945e-7f55b800bca4", null!, 700.50, "Dollar")]
        public async Task GetCardBalance_Success(string identifier, string? country, decimal expectedAmount, string expectedCurrencyCode)
        {
            // Arrange
            // Minimal API url with user input
            var apiUrl = $"/moneymanagement/balance/{identifier}/{country}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var cardBalance = JsonSerializer.Deserialize<CardBalanceModel>(strResponse, options);

            // Assert
            Assert.NotNull(cardBalance);
            Assert.Equal(expectedAmount, cardBalance.Balance);
            Assert.Equal(expectedCurrencyCode, cardBalance.CurrencyCode);
            Assert.Equal(identifier, cardBalance.Identifier);
        }

        [Theory]
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", "Canada", 273.8000, "Dollar")]
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", "Australia", 200.00, "Dollar")]
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", null!, 200.00, "Dollar")]
        public async Task GetTransaction_Success(string identifier, string? country, decimal expectedAmount, string expectedCurrencyCode)
        {
            // Arrange
            // Minimal API url with user input
            var apiUrl = $"/moneymanagement/transaction/{identifier}/{country}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var transaction = JsonSerializer.Deserialize<TransactionModel>(strResponse, options);

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(expectedAmount, transaction.Amount);
            Assert.Equal(expectedCurrencyCode, transaction.CurrencyCode);
            Assert.NotEqual(transaction.CardIdentifier, Guid.Empty);
        }

        [Fact]
        public async Task GetTransaction_TransactionNotFoundInDb_Fail()
        {
            // Arrange
            var identifier = "92876d62-a98a-4fd3-ae29-28cd12a1d5bd";
            var country = "Canada";
            
            // Minimal API url with user input
            var apiUrl = $"/moneymanagement/transaction/{identifier}/{country}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var transaction = JsonSerializer.Deserialize<TransactionModel>(strResponse, options);

            // Assert
            Assert.Null(transaction);
        }

        [Fact]
        public async Task AddTransactionAsync_Success()
        {
            // Arrange
            var transactionCreate = new TransactionCreateModel
            {
                Amount = 10,
                CardId = 1,
                Date = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Description = "AddTransactionAsync Test",
            };
            
            // Minimal API url
            var apiUrl = $"/moneymanagement/transaction";

            // Act
            var response = await _httpClient.PostAsJsonAsync(apiUrl, transactionCreate);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var transaction = JsonSerializer.Deserialize<TransactionSavedModel>(strResponse, options);

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(transactionCreate.Amount, transaction.Amount);
            Assert.Equal(transactionCreate.Description, transaction.Description);
            Assert.Equal(DateTime.Parse(transactionCreate.Date).Date, DateTime.Parse(transaction.Date).Date);
            Assert.NotEqual(transaction.Identifier, Guid.Empty);
        }

        [Fact]
        public async Task AddCardAsync_Success()
        {
            // Arrange
            var cardCreate = new CardCreateModel
            {
                CreditLimit = 10000
            };
            
            // Minimal API url
            var apiUrl = $"/moneymanagement/card";

            // Act
            var response = await _httpClient.PostAsJsonAsync(apiUrl, cardCreate);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var card = JsonSerializer.Deserialize<CardModel>(strResponse, options);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(cardCreate.CreditLimit, card.CreditLimit);
            Assert.True(card.Id > 0);
            Assert.NotEqual(card.Identifier, Guid.Empty);
        }
    }
}
