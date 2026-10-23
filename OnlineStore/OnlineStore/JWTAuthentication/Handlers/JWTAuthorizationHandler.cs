using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using System.Net.Http.Headers; 

namespace OnlineStore.JWTAuthentication.Handlers
{
    public class JWTAuthorizationHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly NavigationManager _navigationManager; 

        public JWTAuthorizationHandler(ITokenProvider tokenProvider, NavigationManager navigationManager)
        {
            _tokenProvider = tokenProvider;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                        CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenProvider.GetToken(); 

            if (!string.IsNullOrEmpty(tokenResult))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _tokenProvider.ClearToken();
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }
    }
}
