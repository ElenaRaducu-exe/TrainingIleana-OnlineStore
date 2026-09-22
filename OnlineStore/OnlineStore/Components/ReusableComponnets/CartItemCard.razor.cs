using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class CartItemCard : ComponentBase
    {
        [Parameter]
        public int Quantity { get; set; }
        [Parameter]
        public string ProductName { get; set; } = string.Empty;
        [Parameter]
        public string Description { get; set; } = string.Empty;
        [Parameter]
        public decimal Price { get; set; }
        [Parameter]
        public string ImageUrl { get; set; } = string.Empty;
        [Parameter]
        public string CategoryName { get; set; } = string.Empty;
        [Parameter]
        public string BrandName { get; set; } = string.Empty;
        [Parameter]
        public int CartId { get; set; }
        [Parameter]
        public int CartItemId { get; set; }
        [Parameter]
        public int ProductId { get; set; }
        [Parameter]
        public int UserId { get; set; }

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
            TotalPrice = Price * Quantity; 
        }

        private async Task UpdateQuantity(int newQuantity)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var cartItemDetails = new CartItemCard
            {
                Quantity = newQuantity
            };

            var response = await httpClient.PutAsJsonAsync($"api/cart/update/quantity/{CartItemId}", cartItemDetails);

            if (response.IsSuccessStatusCode)
            {
                Quantity = newQuantity;

                TotalPrice = Price * Quantity;
                StateHasChanged(); 
            }
        }
    }
}
