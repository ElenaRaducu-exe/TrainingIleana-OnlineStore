using Microsoft.AspNetCore.Components;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class CartProducts : ComponentBase
    {
        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IAddressFrontendService _addressFrontendService { get; set; }

        [Inject]
        private ICartFrontendService _cartFrontendService { get; set; }

        private CartSummaryPageModel? _cartItems { get; set; }


        protected async Task IntializedPageLoadData()
        {
            _cartItems = await _cartFrontendService.GetCartItemsByUser();
        }

        protected override async Task OnInitializedAsync()
        {
            await IntializedPageLoadData();
        }

        public void RedirectToProductDetailsPage(int cartItemId)
        {
            var item = _cartItems?.CartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (item != null)
            {
                _navigation.NavigateTo($"/dashboard/products/{item.ProductId}");
            }
        }

        public void NavigateToProductsPage()
        {
            _navigation.NavigateTo("/dashboard/products");
        }

        protected async Task DeleteCartItem(int cartItemId)
        {
            var response = await _cartFrontendService.DeleteCartItem(cartItemId);

            if (response == true)
            {
                var cartItem = _cartItems?.CartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

                if (cartItem != null)
                {
                    await IntializedPageLoadData();

                    StateHasChanged();
                }
            }
        }

        private async Task HandleQuantityUpdated()
        {
            await IntializedPageLoadData();
        }

        private async Task HandleItemDeleted(int cartItemId)
        {
            var cartItem = _cartItems?.CartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (cartItem != null)
            {
                await DeleteCartItem(cartItemId);
            }
        }

        public void RedirectToAddressForm()
        {
            _navigation.NavigateTo($"place-order/AddressForm/{_cartItems?.UserId}");
        }

        public void RedirectToAddressesPage()
        {
            _navigation.NavigateTo($"/place-order/{_cartItems?.UserId}");
        }

        public async Task AvailableAdresses()
        {
            var addressList = await _addressFrontendService.GetAddressesByUser();

            if (addressList?.Any() == false)
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
            if(_cartItems != null)
            {
                var result = await _cartFrontendService.DeleteCart(_cartItems.CartId);

                if (result == true)
                {
                    _cartItems.CartItems.Clear();
                    _cartItems.TotalCartItems = 0;
                    _cartItems.TotalItemsQuantity = 0;
                    _cartItems.TotalOrderPrice = 0;

                    StateHasChanged();
                }
            }
        }
    }
}
