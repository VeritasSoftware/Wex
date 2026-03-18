namespace Wex.API.Services
{
    public class HttpService : IHttpService
    {
        public HttpClient Client { get; }

        public HttpService(HttpClient client)
        {
            Client = client;
        }
    }
}
