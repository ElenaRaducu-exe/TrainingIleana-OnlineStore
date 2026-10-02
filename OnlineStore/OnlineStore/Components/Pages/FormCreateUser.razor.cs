using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

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

        [Inject]
        private IMapper _mapper { get; set; }

        private UserModel _newUser = new();
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

            var userDTO = _mapper.Map<UserDTO>(_newUser);

            var response = await httpClient.PostAsJsonAsync("api/users", userDTO);

            _apiResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _newUser = new UserModel();

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
