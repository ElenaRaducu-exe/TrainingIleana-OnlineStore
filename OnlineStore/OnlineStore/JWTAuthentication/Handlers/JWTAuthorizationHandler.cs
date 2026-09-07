using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers; 

namespace OnlineStore.JWTAuthentication.Handlers
{
    public class JWTAuthorizationHandler : DelegatingHandler
    {
        private ProtectedSessionStorage _protectedSessionStorage; 

        public JWTAuthorizationHandler(ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                        CancellationToken cancellationToken)
        {
            var tokenResult = await _protectedSessionStorage.GetAsync<string>("authToken");

            if(tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
