using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class UsersDashboard : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private  ITokenProvider _tokenProvider { get; set; }

        public List<User> UsersList = new();

        /*
        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("AuthenticatedUser");
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            UsersList = await httpClient.GetFromJsonAsync<List<User>>("api/users/dashboard");
        }*/

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var token = await _tokenProvider.GetToken(); 

                var httpClient = _httpClientFactory.CreateClient("AuthenticatedUser");
                httpClient.BaseAddress = new Uri(_navigation.BaseUri);

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                UsersList = await httpClient.GetFromJsonAsync<List<User>>("api/users/dashboard");

                StateHasChanged(); 
            }
        }

        private async Task UpdateActiveMode(int id)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient("AuthenticatedUser");
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var user = await httpClient.GetFromJsonAsync<User>($"api/users/dashboard/{id}");

            var response = await httpClient.PutAsJsonAsync($"api/users/dashboard/update/active/{id}", user);

            if (response.IsSuccessStatusCode)
            {
                var updatedUser = await response.Content.ReadFromJsonAsync<User>();

                if (updatedUser != null)
                {
                    var userList = UsersList.FirstOrDefault(u => u.Id == id);

                    if (userList != null)
                    {
                        userList.IsActive = updatedUser.IsActive;
                    }

                    StateHasChanged();
                }
            }
        }
    }
}
