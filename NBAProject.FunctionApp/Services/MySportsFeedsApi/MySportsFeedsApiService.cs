using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace NBAProject.FunctionApp.Services.MySportsFeedsApi
{
    public class MySportsFeedsApiService : IMySportsFeedsApiService
    {
        private readonly RestClient _restClient;

        public MySportsFeedsApiService(IOptions<MySportsFeedsApiSettings> settings)
        {
            var apiSettings = settings.Value;

            _restClient = new RestClient
            {
                BaseUrl = new Uri(apiSettings.BaseUrl),
                Authenticator = new HttpBasicAuthenticator(apiSettings.Username, apiSettings.Password)
            };
        }

        public async Task<string> GetAsync(RestRequest restRequest)
        {
            var response = await _restClient.ExecuteGetAsync(restRequest);

            return response.Content;
        }
    }

    public interface IMySportsFeedsApiService
    {
        public Task<string> GetAsync(RestRequest restRequest);
    }
}