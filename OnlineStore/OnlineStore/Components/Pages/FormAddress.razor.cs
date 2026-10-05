using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddress : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        private AddressModel _addressModel { get; set; } = new();
        private string _messageAddedAddressError = string.Empty;
        private bool? _addressAddedSucces;

        private async Task AddAdress()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            AddressDTO addressDTO = _mapper.Map<AddressDTO>(_addressModel);

            var response = await httpClient.PostAsJsonAsync("api/address/add", addressDTO);

            if (response.IsSuccessStatusCode)
            {
                _messageAddedAddressError = "Address added successfully!";
                _addressAddedSucces = true;

                await Task.Delay(2000);

                _navigation.NavigateTo("/place-order/addresses");
            }
            else
            {
                _messageAddedAddressError = "Address added unsuccessfully!";
                _addressAddedSucces = false;
            }
        }
    }
}
