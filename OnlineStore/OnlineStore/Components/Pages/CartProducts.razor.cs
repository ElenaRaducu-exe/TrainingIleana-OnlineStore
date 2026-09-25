using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class CartProducts : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private List<CartItemModel> _cartItems { get; set; }
        private decimal _totalOrderPrice { get; set; }
        private int _totalCartItems { get; set; }

        protected async Task IntializedPageLoadData()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var cartItemDTO = await httpClient.GetFromJsonAsync<List<CartItemDTO>>("api/cart/items/user");

            if (cartItemDTO != null)
            {
                _cartItems = _mapper.Map<List<CartItemModel>>(cartItemDTO);
            }

            _totalOrderPrice = _cartItems.Sum(item => item.Price * item.Quantity);

            _totalCartItems = _cartItems.Count();
        }

        protected override async Task OnInitializedAsync()
        {
            await IntializedPageLoadData();
        }

        public void RedirectToProductDetailsPage(int cartItemId)
        {
            var item = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);
            _navigation.NavigateTo($"/dashboard/products/{item.ProductId}");
        }

        public void NavigateToProductsPage()
        {
            _navigation.NavigateTo("/dashboard/products");
        }

        protected async Task DeleteCartItem(int cartId, int cartItemId)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var cart = _cartItems.FirstOrDefault(cart => cart.CartId == cartId);
            var cartItem = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            var response = await httpClient.DeleteAsync($"api/cart/delete/{cartItemId}");

            if (response.IsSuccessStatusCode && cartItem != null)
            {
                _cartItems.Remove(cartItem);

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
    }
}
