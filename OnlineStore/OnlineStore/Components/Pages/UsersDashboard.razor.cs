using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class UsersDashboard : ComponentBase
    {
        [Inject]
        private IUserFrontendService _userFrontendService { get; set; }

        public List<User> UsersList = new();

        
        protected override async Task OnInitializedAsync()
        {
            UsersList = await _userFrontendService.GetUsers();
        }

        private async Task UpdateActiveMode(User user)
        {
            var responseUpdatingStatusUser = await _userFrontendService.UpdateUserActiveMode(user);

            if (responseUpdatingStatusUser == true)
            {
                user.IsActive = !user.IsActive;

                StateHasChanged();
            }
        }
    }
}
