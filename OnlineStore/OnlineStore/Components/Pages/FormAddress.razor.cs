using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddress : ComponentBase
    {
        [Inject]
        private IApiClient _apiClient { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private AddressModel _addressModel { get; set; } = new();
        private string _messageAddedAddressError = string.Empty;
        private bool? _addressAddedSucces;

        [Parameter]
        public int? UserId { get; set; }

        private async Task AddAdress()
        {
            AddressDTO addressDTO = _mapper.Map<AddressDTO>(_addressModel);

            var httpClient = await _apiClient.GetHttpClientAsync();

            var response = await httpClient.PostAsJsonAsync("api/address/add", addressDTO);

            if (response.IsSuccessStatusCode)
            {
                _messageAddedAddressError = "Address added successfully!";
                _addressAddedSucces = true;

                await Task.Delay(2000);

                _navigation.NavigateTo($"/place-order/{UserId}");
            }
            else
            {
                _messageAddedAddressError = "Address added unsuccessfully!";
                _addressAddedSucces = false;
            }
        }
    }
}
