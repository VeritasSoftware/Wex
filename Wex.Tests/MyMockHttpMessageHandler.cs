using System.Net;
using System.Text;

namespace Wex.Tests
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
}
