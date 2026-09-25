using Microsoft.AspNetCore.Components;
using OnlineStore.Data;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class CartItemCard : ComponentBase
    {
        [Parameter]
        public CartItemModel CartItemModel { get; set; } 

        private int _quantity { get; set; }

        private int _availableStock { get; set; }

        [Parameter]
        public EventCallback OnQuantityUpdated { get; set; }

        [Parameter]
        public EventCallback<int> OnItemSelected { get; set; }

        [Parameter]
        public EventCallback<int> OnItemDeleted { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public decimal TotalPrice { get; set; }

        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TotalPrice = CartItemModel.Price * CartItemModel.Quantity;
            _quantity = CartItemModel.Quantity;

            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // api/admin/products/{productId:int}
            _availableStock = httpClient.GetFromJsonAsync<ProductDTO>($"api/admin/products/{CartItemModel.ProductId}").Result.Stock;
        }

        private async Task IncreaseQuantity(int quantity)
        {
            quantity += 1; 
            await UpdateQuantity(quantity);
        }
        private async Task DecreaseQuantity(int quantity)
        {
            quantity -= 1; 
            await UpdateQuantity(quantity);
        }

        private async Task UpdateQuantity(int newQuantity)
        {
            if(newQuantity < 0)
            {
                return;
            }

            if(newQuantity == 0)
            {
                await OnItemDeleted.InvokeAsync(CartItemModel.CartItemId);
                return;
            }

            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PutAsJsonAsync($"api/cart/update/quantity/{CartItemModel.CartItemId}", newQuantity);

            if (response.IsSuccessStatusCode)
            {
                _quantity = newQuantity;

                TotalPrice = CartItemModel.Price * _quantity;

                _availableStock = httpClient.GetFromJsonAsync<ProductDTO>($"api/admin/products/{CartItemModel.ProductId}").Result.Stock;

                StateHasChanged();

                await OnQuantityUpdated.InvokeAsync();
            }
        }

        private async Task CartItemClicked(int cartItemId)
        {
            await OnItemSelected.InvokeAsync(cartItemId);
        }
    }
}
