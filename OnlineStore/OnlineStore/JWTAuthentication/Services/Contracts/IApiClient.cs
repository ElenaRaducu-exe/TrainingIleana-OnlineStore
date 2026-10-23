namespace OnlineStore.JWTAuthentication.Services.Contracts
{
    public interface IApiClient
    {
        Task<HttpClient> GetHttpClientAsync();

        Task<T?> GetAsync<T>(string requestUri);
        Task<HttpResponseMessage> GetRawAsync(string requestUri);
        Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string requestUri, TRequest content);
        Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string requestUri, TRequest content);
        Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(string requestUri, TRequest content);
        Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(string requestUri, TRequest content);
        Task<HttpResponseMessage> DeleteAsync(string requestUri); 
    }
}
