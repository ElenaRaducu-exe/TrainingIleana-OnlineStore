using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddress : ComponentBase
    {
        [Inject]
        private IAddressFrontendService _addressFrontendService { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        private AddressModel _addressModel { get; set; } = new();
        private string _messageAddedAddressError = string.Empty;
        private bool? _addressAddedSucces;

        [Parameter]
        public int? UserId { get; set; }

        private async Task AddAdress()
        {
            _addressAddedSucces = await _addressFrontendService.AddAddressAsync(_addressModel);

            if (_addressAddedSucces == true)
            {
                _messageAddedAddressError = "Address added successfully!";

                await Task.Delay(2000);

                _navigation.NavigateTo($"/place-order/{UserId}");
            }
            else
            {
                _messageAddedAddressError = "Address added unsuccessfully!";
            }
        }
    }
}
