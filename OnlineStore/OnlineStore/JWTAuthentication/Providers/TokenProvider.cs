using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OnlineStore.JWTAuthentication.Providers.Contracts;

namespace OnlineStore.JWTAuthentication.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public TokenProvider(ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }

        public async Task ClearToken()
        {
            await _protectedSessionStorage.DeleteAsync("authToken");
        }

        public async Task<string?> GetToken()
        {
            try
            {
                var token = await _protectedSessionStorage.GetAsync<string>("authToken");

                if (!token.Success)
                {
                    return null;
                }

                return token.Value;
            }
            catch
            {
                return null; 
            }
        }

        public async Task SetToken(string token)
        {
            await _protectedSessionStorage.SetAsync("authToken", token);
        }
    }
}
