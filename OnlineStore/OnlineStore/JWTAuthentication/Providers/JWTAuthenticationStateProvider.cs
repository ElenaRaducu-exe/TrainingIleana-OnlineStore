using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineStore.JWTAuthentication.Providers
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenProvider _tokenProvider;

        private string? _token {  get; set; }

        public JWTAuthenticationStateProvider(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public string? GetToken()
        {
            return _token;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            try
            {
                var token = await _tokenProvider.GetToken();

                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(user);
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtToken = tokenHandler.ReadJwtToken(token);

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
            _token = token;

            await _tokenProvider.SetToken(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            _token = null;

            await _tokenProvider.ClearToken();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
