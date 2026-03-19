using Microsoft.AspNetCore.Mvc.Testing;
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
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", "Canada", 273.8000, "Dollar")]
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", "Australia", 200.00, "Dollar")]
        [InlineData("3d142c0f-1fc9-48cf-be15-26eea3497b71", null!, 200.00, "Dollar")]
        public async Task GetTransaction_Success(string identifier, string? country, decimal expectedAmount, string expectedCurrencyCode)
        {
            // Arrange
            // Minimal API url with user input
            var apiUrl = $"/moneymanagement/{identifier}/{country}";

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
        }

        [Fact]
        public async Task GetTransaction_TransactionNotFoundInDb_Fail()
        {           
            var identifier = "92876d62-a98a-4fd3-ae29-28cd12a1d5bd";
            var country = "Canada";

            // Arrange
            // Minimal API url with user input
            var apiUrl = $"/moneymanagement/{identifier}/{country}";

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
    }
}
