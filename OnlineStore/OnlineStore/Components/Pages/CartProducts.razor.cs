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

        private List<CartItemModel>? _cartItems { get; set; }
        private decimal _totalOrderPrice { get; set; }
        private int _totalCartItems { get; set; }
        private int _userID { get; set; }
        private int _cartId { get; set; }

        protected async Task IntializedPageLoadData()
        {
            _cartItems = await _cartFrontendService.GetCartItemsByUser();

            if (_cartItems != null && _cartItems.Count() > 0)
            {
                _userID = _cartItems.First().UserId;

                _cartId = _cartItems.First().CartId;

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

        protected async Task DeleteCartItem(int cartItemId)
        {
            var response = await _cartFrontendService.DeleteCartItem(cartItemId);

            if (response == true)
            {
                var cartItem = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

                if (cartItem != null)
                {
                    _cartItems.Remove(cartItem);
                    _totalCartItems = _cartItems.Count();

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
            var cartItem = _cartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (cartItem != null)
            {
                await DeleteCartItem(cartItemId);
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
            var result = await _cartFrontendService.DeleteCart(_cartId);

            if (result == true)
            {
                _cartItems.Clear();
                _totalCartItems = 0;
                _totalOrderPrice = 0;

                StateHasChanged();
            }
        }
    }
}
