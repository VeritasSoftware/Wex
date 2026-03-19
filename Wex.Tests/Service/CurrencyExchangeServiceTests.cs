using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Net;
using System.Text.Json;
using Wex.API.Models;
using Wex.API.Services;

namespace Wex.Tests.Service
{
    public class CurrencyExchangeServiceTests
    {
        [Fact]
        public async Task ConvertCurrencyAsync_Should_Return_Correct_Amount()
        {
            // Arrange
            decimal amountToConvert = 100.00m;
            string country = "Canada";

            var httpServiceMock = Substitute.For<IHttpService>();
            var configurationMock = Substitute.For<IConfiguration>();

            configurationMock.GetValue<string>("CurrencyExchangeAPIBaseUrl")
                             .Returns("https://api.fiscaldata.treasury.gov/services/api/fiscal_service");

            configurationMock.GetValue<string>("CurrencyExchangeAPIEndpoint")
                             .Returns("/v1/accounting/od/rates_of_exchange");

            var exchangeRateResponse =
                new CurrencyExchangeResponse
                {
                    Data = new List<ExchangeRateModel>
                                {
                                    new ExchangeRateModel
                                    {
                                        Country = "Canada",
                                        CurrencyCode = "Dollar",
                                        CountryCurrencyDesc = "Canada-Dollar",
                                        ExchangeRate = "1.252",
                                        RecordDate = DateOnly.FromDateTime(DateTime.Now)
                                    }
                                }
                };            

            var jsonResponse = JsonSerializer.Serialize(exchangeRateResponse);

            var handler = new MyMockHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service")
            }; 
            
            httpServiceMock.Client.Returns(client);

            var currencyExchangeService = new CurrencyExchangeService(httpServiceMock, configurationMock);

            // Act
            var exchangeRate = await currencyExchangeService.ConvertCurrencyAsync(DateOnly.FromDateTime(DateTime.Now), country);

            // Assert
            Assert.NotNull(exchangeRate);
            Assert.Equal(125.200m, decimal.Parse(exchangeRate.ExchangeRate) * amountToConvert);
        }
    }

}
