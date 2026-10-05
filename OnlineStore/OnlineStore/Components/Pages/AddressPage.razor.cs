using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class AddressPage : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private List<AddressModel> addressList { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var addressDTOList = await httpClient.GetFromJsonAsync<List<AddressDTO>>("api/address");

            addressList = _mapper.Map<List<AddressModel>>(addressDTOList);
        }

        public void RedirectToAddressForm()
        {
            _navigation.NavigateTo("/place-order/AddressForm");
        }
    }
}
