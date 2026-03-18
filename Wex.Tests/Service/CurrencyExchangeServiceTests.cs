using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Wex.API.Models;
using Wex.API.Services;

namespace Wex.Tests.Service
{
    public class MyMockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public MyMockHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            });
        }
    }

    public class CurrencyExchangeServiceTests
    {
        [Fact]
        public async Task ConvertCurrencyAsync_Should_Return_Correct_Amount()
        {
            // Arrange
            var httpServiceMock = Substitute.For<IHttpService>();
            var configurationMock = Substitute.For<IConfiguration>();

            configurationMock.GetValue<string>("CurrencyExchangeAPIBaseUrl")
                             .Returns("https://api.fiscaldata.treasury.gov/services/api/fiscal_service");

            configurationMock.GetValue<string>("CurrencyExchangeAPIEndpoint")
                             .Returns("/v1/accounting/od/rates_of_exchange");

            var exchangeRateResponse =
                new CurrencyExchangeResponse
                {
                    Data = new List<Data>
                                {
                                    new Data
                                    {
                                        CountryCurrencyDesc = "Canada Dollar",
                                        ExchangeRate = "1.252",
                                        RecordDate = DateOnly.FromDateTime(DateTime.Now)
                                    }
                                }
                };

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true, // not necessary, but more readable
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonResponse = JsonSerializer.Serialize(exchangeRateResponse, jsonSerializerOptions);

            var handler = new MyMockHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service")
            }; 
            
            httpServiceMock.Client.Returns(client);

            var currencyExchangeService = new CurrencyExchangeService(httpServiceMock, configurationMock);

            // Act
            var result = await currencyExchangeService.ConvertCurrencyAsync(100m, DateOnly.FromDateTime(DateTime.Now), "Canada");

            // Assert
            Assert.Equal(125.200m, result);
        }
    }

}
