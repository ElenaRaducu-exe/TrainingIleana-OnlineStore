using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services.Contracts;
using System.Net.Http.Headers; 

namespace OnlineStore.JWTAuthentication.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient; 
        private readonly ITokenProvider _tokenProvider;
        private readonly NavigationManager _navigationManager; 

        public ApiClient(HttpClient httpClient, ITokenProvider tokenProvider, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _navigationManager = navigationManager;

            _httpClient.BaseAddress = new Uri(_navigationManager.BaseUri);
        }

        public async Task<HttpClient> GetHttpClientAsync()
        {
            var token = await _tokenProvider.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _httpClient;
        }

    }
}
