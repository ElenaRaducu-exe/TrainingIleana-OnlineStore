using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
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
        private List<CartItemModel> _cartItemsList { get; set; }

        [Parameter]
        public int? UserId { get; set; }

        public int CartId { get; set; }
        public int? AddressId { get; set; }
        private bool? _orderPlaceSuccess = false;

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var addressDTOList = await httpClient.GetFromJsonAsync<List<AddressDTO>>("api/address");

            addressList = _mapper.Map<List<AddressModel>>(addressDTOList);

            var cartItemDTO = await httpClient.GetFromJsonAsync<List<CartItemDTO>>("api/cart/items/user");

            if (cartItemDTO != null && cartItemDTO.Count() > 0)
            {
                CartId = cartItemDTO.First().CartId;
                _cartItemsList = _mapper.Map<List<CartItemModel>>(cartItemDTO);
            }
        }

        public void RedirectToAddressForm()
        {
            _navigation.NavigateTo($"/place-order/AddressForm/{UserId}");
        }

        public void RedirectToProductDetailsPage(int cartItemId)
        {
            var item = _cartItemsList.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (item != null)
            {
                _navigation.NavigateTo($"/dashboard/products/{item.ProductId}");
            }
        }

        public void GetSelectedAddressId(int id)
        {
            AddressId = id;
            StateHasChanged();
        }

        public async Task PlaceOrder()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var orderItemDTO = new OrderItemDTO();

            if(AddressId != null)
            {
                var result = await httpClient.PostAsJsonAsync($"api/order/create/{AddressId}/{UserId}/{CartId}", orderItemDTO);

                _cartItemsList.Clear();

                StateHasChanged();

                _orderPlaceSuccess = true;
            }
        }
    }
}
