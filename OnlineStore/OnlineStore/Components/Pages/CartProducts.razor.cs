using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class CartProducts : ComponentBase
    {
        [Inject]
        private IApiClient _apiClient { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private List<CartItemModel> _cartItems { get; set; }
        private decimal _totalOrderPrice { get; set; }
        private int _totalCartItems { get; set; }
        private int _userID { get; set; }
        private int _cartId { get; set; }

        protected async Task IntializedPageLoadData()
        {
            var httpClient = await _apiClient.GetHttpClientAsync();
            var cartItemDTO = await httpClient.GetFromJsonAsync<List<CartItemDTO>>("api/cart/items/user");

            if (cartItemDTO != null && cartItemDTO.Count() > 0)
            {
                _cartItems = _mapper.Map<List<CartItemModel>>(cartItemDTO);

                _userID = cartItemDTO.First().UserId;

                _cartId = cartItemDTO.First().CartId;

                _totalOrderPrice = _cartItems.Sum(item => item.Price * item.Quantity);

                _totalCartItems = _cartItems.Count();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await IntializedPageLoadData();
        }

        public void RedirectToProductDetailsPage(int cartItemId)
        {
            var item = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (item != null)
            {
                _navigation.NavigateTo($"/dashboard/products/{item.ProductId}");
            }
        }

        public void NavigateToProductsPage()
        {
            _navigation.NavigateTo("/dashboard/products");
        }

        protected async Task DeleteCartItem(int cartId, int cartItemId)
        {
            var cart = _cartItems.FirstOrDefault(cart => cart.CartId == cartId);
            var cartItem = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            var httpClient = await _apiClient.GetHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/cart/delete/{cartItemId}");

            if (response.IsSuccessStatusCode && cartItem != null)
            {
                _cartItems.Remove(cartItem);
                _totalCartItems = _cartItems.Count();

                StateHasChanged();
            }
        }

        private async Task HandleQuantityUpdated()
        {
            await IntializedPageLoadData();
        }

        private async Task HandleItemDeleted(int cartItemId)
        {
            var cartItem = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (cartItem != null)
            {
                await DeleteCartItem(cartItem.CartId, cartItemId);
            }
        }

        public void RedirectToAddressForm()
        {
            _navigation.NavigateTo($"place-order/AddressForm/{_userID}");
        }

        public void RedirectToAddressesPage()
        {
            _navigation.NavigateTo($"/place-order/{_userID}");
        }

        public async Task AvailableAdresses()
        {
            var httpClient = await _apiClient.GetHttpClientAsync();
            var addressDTO = await httpClient.GetFromJsonAsync<List<AddressDTO>>("api/address");

            if(addressDTO?.Any() == false)
            {
                RedirectToAddressForm();
            }
            else
            {
                RedirectToAddressesPage();
            }
        }

        public async Task DeleteCart()
        {
            var httpClient = await _apiClient.GetHttpClientAsync();
            var result = await httpClient.DeleteAsync($"api/cart/delete/cart/{_cartId}");

            if(result.IsSuccessStatusCode)
            {
                _cartItems.Clear();
                _totalCartItems = 0;
                _totalOrderPrice = 0;

                StateHasChanged();
            }
        }
    }
}
