using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class FormCreateUser : ComponentBase
    {
        [Inject]
        private IApiClient _apiClient { get; set; }

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

            var userDTO = _mapper.Map<UserDTO>(_newUser); // serv front

            var httpClient = await _apiClient.GetHttpClientAsync();

            var response = await httpClient.PostAsJsonAsync("api/users", userDTO);

            //_apiResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _newUser = new UserModel();

                _repetedPassword = string.Empty;
                _passwordMatchError = false;
            }
            else
            {
                // Handle error response + messages on UI
                _usernameExistingError = true;
            }
        }
    }
}
