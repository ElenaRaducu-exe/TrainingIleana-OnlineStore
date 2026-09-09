using Microsoft.AspNetCore.Components;
using OnlineStore.Models;

namespace OnlineStore.Components.Pages
{
    public partial class FormCreateUser : ComponentBase
    {
        [Inject]
        private HttpClient _httpClient { get; set; }

        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        private CreateUserDTO _newUser = new();
        private string _repetedPassword = string.Empty;
        private string _apiResponse = string.Empty;
        private bool _usernameExistingError = false;
        private bool _passwordMatchError = false;

        private async Task CreateUser()
        {
            _usernameExistingError = false;
            _passwordMatchError = false;

            if (_newUser.Password != _repetedPassword)
            {
                _passwordMatchError = true;
                return;
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            //var response = await _httpClient.PostAsJsonAsync("api/users", _newUser);
            var response = await httpClient.PostAsJsonAsync("api/users", _newUser);

            _apiResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _newUser = new CreateUserDTO();

                _repetedPassword = string.Empty;
                _passwordMatchError = false;
            }
            else
            {
                _usernameExistingError = true;
            }
        }
    }
}
