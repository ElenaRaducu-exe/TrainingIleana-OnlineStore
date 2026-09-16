using Microsoft.AspNetCore.Components;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class CartProducts : ComponentBase
    {
        [Inject]
        private NavigationManager _navigation { get; set; }

        private List<ProductDTO> _products { get; set; }

        [Inject]
        private ICartProductsService _cartService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _products = await _cartService.GetCartProducts();
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
