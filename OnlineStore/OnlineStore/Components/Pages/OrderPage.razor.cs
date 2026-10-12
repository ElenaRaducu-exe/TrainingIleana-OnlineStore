using AutoMapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class OrderPage : ComponentBase
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
        private List<CartItemModel> _cartItemsList { get; set; } = new();

        [Parameter]
        public int? UserId { get; set; }
        private bool? _orderPlaceSuccess = false;

        private CreateOrderRequestDTO _createOrderRequestDTO { get; set; } = new CreateOrderRequestDTO();

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
                _createOrderRequestDTO.CartId = cartItemDTO.First().CartId;
                _cartItemsList = _mapper.Map<List<CartItemModel>>(cartItemDTO);
            }

            if (UserId != null)
            {
                _createOrderRequestDTO.UserId = UserId.Value;
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
            _createOrderRequestDTO.AddressId = id;

            StateHasChanged();
        }

        public async Task PlaceOrder()
        {
            if (_createOrderRequestDTO.AddressId != null)
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_navigation.BaseUri);

                CreateOrderRequestDTO createOrderRequestDTO = new CreateOrderRequestDTO()
                {
                    AddressId = _createOrderRequestDTO.AddressId,
                    UserId = _createOrderRequestDTO.UserId,
                    CartId = _createOrderRequestDTO.CartId
                };
            
                var result = await httpClient.PostAsJsonAsync($"api/order/create", createOrderRequestDTO);

                _cartItemsList.Clear();

                StateHasChanged();

                _orderPlaceSuccess = true;
            }
        }
    }
}
