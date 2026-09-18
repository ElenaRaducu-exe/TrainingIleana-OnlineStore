using AutoMapper;
using Microsoft.AspNetCore.Components;
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
        private ICartProductsService _cartService { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        private List<CartItemModel> _cartItems { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var cartItemDTO = await httpClient.GetFromJsonAsync<List<CartItemDTO>>("api/cart/items/user");

            if(cartItemDTO != null)
            {
                _cartItems = _mapper.Map<List<CartItemModel>>(cartItemDTO);
            }
        }

        public void RedirectToProductDetailsPage(int id)
        {
            _navigation.NavigateTo($"/dashboard/products/{id}");
        }

        public void NavigateToProductsPage()
        {
            _navigation.NavigateTo("/dashboard/products");
        }
    }
}
