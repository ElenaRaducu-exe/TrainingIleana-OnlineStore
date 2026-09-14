namespace OnlineStore.JWTAuthentication.Providers.Contracts
{
    public interface ITokenProvider
    {
        Task SetToken(string token); 
        Task<string?> GetToken();
        Task ClearToken();
    }
}
