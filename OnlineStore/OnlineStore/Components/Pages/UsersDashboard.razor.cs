using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;

namespace OnlineStore.Components.Pages
{
    public partial class UsersDashboard : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        public List<User> UsersList = new();

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            UsersList = await httpClient.GetFromJsonAsync<List<User>>("api/users/dashboard");
        }

        private async Task UpdateActiveMode(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

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
