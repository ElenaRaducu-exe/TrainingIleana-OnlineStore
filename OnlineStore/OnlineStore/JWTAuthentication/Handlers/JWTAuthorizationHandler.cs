using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.Net.Http.Headers; 

namespace OnlineStore.JWTAuthentication.Handlers
{
    public class JWTAuthorizationHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public JWTAuthorizationHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                        CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenProvider.GetToken(); 

            if (!string.IsNullOrEmpty(tokenResult))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
