using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services.Contracts;
using System.Net;
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
                // token JWT attached to httpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _httpClient;
        }

        // refactor ApiClient => http methods (GET, POST, PUT, DELETE) with token handling (JWT token attached to the request)
        private async Task<HttpResponseMessage> SendHttpRequestJwtTokenAttachedAsync(HttpMethod method, string requestUriEnpoint, HttpContent? content = null)
        {
            var httpRequest = new HttpRequestMessage(method, requestUriEnpoint); 

            if(content != null)
            {
                httpRequest.Content = content;
            }

            var token = await _tokenProvider.GetToken();

            if(!string.IsNullOrEmpty(token))
            {
                // token JWT attached to each http method request 
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenProvider.ClearToken();
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }

        // GET -> READ
        public async Task<T?> GetAsync<T>(string requestUri)
        {
            var response = await SendHttpRequestJwtTokenAttachedAsync(HttpMethod.Get, requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            return default;
        }

        public async Task<HttpResponseMessage> GetRawAsync(string requestUri)
        {
            return await SendHttpRequestJwtTokenAttachedAsync(HttpMethod.Get, requestUri);
        }

        // POST -> CREATE
        public async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string requestUri, TRequest content)
        {
            return await SendHttpRequestJwtTokenAttachedAsync(HttpMethod.Post, requestUri, JsonContent.Create(content)); 
        }

        public async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string requestUri, TRequest content)
        {
            var response = await PostAsJsonAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(); 
            }

            return default; 
        }

        // PUT => UPDATE/EDIT 
        public async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(string requestUri, TRequest content)
        {
            return await SendHttpRequestJwtTokenAttachedAsync(HttpMethod.Put, requestUri, JsonContent.Create(content));
        }

        public async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(string requestUri, TRequest content)
        {
            var response = await PutAsJsonAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            return default; 
        }

        // DELETE
        public Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return SendHttpRequestJwtTokenAttachedAsync(HttpMethod.Delete, requestUri); 
        }
    }
}
