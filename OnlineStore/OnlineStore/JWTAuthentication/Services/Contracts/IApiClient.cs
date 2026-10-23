namespace OnlineStore.JWTAuthentication.Services.Contracts
{
    public interface IApiClient
    {
        Task<HttpClient> GetHttpClientAsync();
    }
}
