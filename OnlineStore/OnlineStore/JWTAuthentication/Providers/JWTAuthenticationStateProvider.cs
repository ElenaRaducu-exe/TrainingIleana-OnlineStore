using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineStore.JWTAuthentication.Providers
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public JWTAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            try
            {
                var token = await _sessionStorage.GetAsync<string>("authToken");

                if (string.IsNullOrEmpty(token.Value) || !token.Success)
                {
                    return new AuthenticationState(user);
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtToken = tokenHandler.ReadJwtToken(token.Value);

                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");

                var autheticatedUser = new ClaimsPrincipal(identity);

                return new AuthenticationState(autheticatedUser);
            }
            catch
            {
                return new AuthenticationState(user);
            }
        }

        public async Task LoginAsync(string token)
        {
            await _sessionStorage.SetAsync("authToken", token); 
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await _sessionStorage.DeleteAsync("authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
