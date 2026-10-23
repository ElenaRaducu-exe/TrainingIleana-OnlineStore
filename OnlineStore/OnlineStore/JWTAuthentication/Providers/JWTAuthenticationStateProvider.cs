using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineStore.JWTAuthentication.Providers
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IConfiguration _configuration;

        public JWTAuthenticationStateProvider(ITokenProvider tokenProvider, IConfiguration configuration)
        {
            _tokenProvider = tokenProvider;
            _configuration = configuration;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

            var token = await _tokenProvider.GetToken();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(anonymousUser);
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = JwtTokenValidationOptionsFactory.CreateTokenValidationParameters(_configuration);

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _); 

                return new AuthenticationState(principal);
            }
            catch(SecurityTokenException)
            {
                await _tokenProvider.ClearToken();
                return new AuthenticationState(anonymousUser);
            }
        }

        public async Task LoginAsync(string token)
        {
            await _tokenProvider.SetToken(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await _tokenProvider.ClearToken();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
