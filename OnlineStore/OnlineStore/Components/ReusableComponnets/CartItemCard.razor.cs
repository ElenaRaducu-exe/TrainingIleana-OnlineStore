using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
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
        }

        private async Task UpdateQuantity(int newQuantity)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PutAsJsonAsync($"api/cart/update/quantity/{CartItemModel.CartItemId}", newQuantity);

            if (response.IsSuccessStatusCode)
            {
                _quantity = newQuantity;

                TotalPrice = CartItemModel.Price * _quantity;
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
