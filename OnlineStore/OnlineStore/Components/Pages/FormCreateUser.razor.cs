using Microsoft.AspNetCore.Components;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class FormCreateUser : ComponentBase
    {
        [Inject]
        private IUserFrontendService _userFrontendService { get; set; }

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

            var responseAddingUser = await _userFrontendService.CreateUser(_newUser);

            if (responseAddingUser)
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
