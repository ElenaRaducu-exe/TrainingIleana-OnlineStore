using Microsoft.AspNetCore.Components;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services.Contracts;
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
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IApiClient _apiClient { get; set; }

        private ProductDTO _productDTO { get; set; } = new ProductDTO();

        private bool _disabledIncreaseQuantityBtn = false;

        protected override async Task OnInitializedAsync()
        {
            TotalPrice = CartItemModel.Price * CartItemModel.Quantity;
            _quantity = CartItemModel.Quantity;

            var httpClient = await _apiClient.GetHttpClientAsync();

            var resultProduct = httpClient.GetFromJsonAsync<ProductDTO>($"api/admin/products/get/product/{CartItemModel.ProductId}").Result;

            if( resultProduct != null)
            {
                _productDTO = resultProduct;
            }

            if(_productDTO.ReservedStock == _productDTO.Stock)
            {
                _disabledIncreaseQuantityBtn = true; 
            }
        }

        private async Task IncreaseQuantity()
        {   
            int increasedQuantity = _quantity + 1;

            if (_productDTO.ReservedStock >= _productDTO.Stock)
            {
                _disabledIncreaseQuantityBtn = true;
                return;
            }

            var httpClient = await _apiClient.GetHttpClientAsync();

            var responseCart = await httpClient.PutAsJsonAsync($"api/cart/update/quantity/{CartItemModel.CartItemId}", increasedQuantity);

            if (responseCart.IsSuccessStatusCode)
            {
                _quantity = increasedQuantity;

                CartItemModel.Quantity = _quantity;

                TotalPrice = CartItemModel.Price * _quantity;

                _disabledIncreaseQuantityBtn = _productDTO.ReservedStock >= _productDTO.Stock;

                StateHasChanged();

                await OnQuantityUpdated.InvokeAsync();
            }
        }

        private async Task DecreaseQuantity()
        {
            int decreasedQuantity = _quantity - 1;

            if (decreasedQuantity < 0)
            {
                return;
            }

            if (decreasedQuantity == 0)
            {
                await OnItemDeleted.InvokeAsync(CartItemModel.CartItemId);
                return;
            }

            var httpClient = await _apiClient.GetHttpClientAsync();

            var responseCart = await httpClient.PutAsJsonAsync($"api/cart/update/quantity/{CartItemModel.CartItemId}", decreasedQuantity);

            if (responseCart.IsSuccessStatusCode)
            {
                _quantity = decreasedQuantity;

                CartItemModel.Quantity = _quantity;

                TotalPrice = CartItemModel.Price * _quantity;

                _disabledIncreaseQuantityBtn = _productDTO.ReservedStock >= _productDTO.Stock;

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
