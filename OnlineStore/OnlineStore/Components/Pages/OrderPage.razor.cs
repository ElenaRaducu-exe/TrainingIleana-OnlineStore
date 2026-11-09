using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class OrderPage : ComponentBase
    {
        [Parameter] public int? UserId { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IAddressFrontendService _addressFrontendService { get; set; }

        [Inject]
        private IOrderFrontendService  _orderFrontendService { get; set; }

        [Inject]
        private ICartFrontendService _cartFrontendService { get; set; }

        private List<AddressModel>? addressList { get; set; } = new();
        private CartSummaryPageModel? _cartItemsList { get; set; } = new();

        private bool? _orderPlaceSuccess = false;

        private CreateOrderRequestDTO _createOrderRequestDTO { get; set; } = new CreateOrderRequestDTO();

        protected override async Task OnInitializedAsync()
        {
            addressList = await _addressFrontendService.GetAddressesByUser();

            _cartItemsList = await _cartFrontendService.GetCartItemsByUser();

            if (_cartItemsList?.TotalCartItems > 0)
            {
                _createOrderRequestDTO.CartId = _cartItemsList.CartId;
            }

            if (UserId != null)
            {
                _createOrderRequestDTO.UserId = UserId.Value;
            }
        }

        public void RedirectToAddressForm()
        {
            _navigation.NavigateTo($"/place-order/AddressForm/{UserId}");
        }

        public void RedirectToProductDetailsPage(int cartItemId)
        {
            var item = _cartItemsList?.CartItems.FirstOrDefault(item => item.CartItemId == cartItemId);

            if (item != null)
            {
                _navigation.NavigateTo($"/dashboard/products/{item.ProductId}");
            }
        }

        public void GetSelectedAddressId(int id)
        {
            _createOrderRequestDTO.AddressId = id;

            StateHasChanged();
        }

        public async Task PlaceOrder()
        {
            var response = await _orderFrontendService.PlaceOrder(_createOrderRequestDTO);

            if(response == true)
            {
                _cartItemsList?.CartItems.Clear();

                StateHasChanged();

                _orderPlaceSuccess = true;
            }
            else
            {
                _orderPlaceSuccess = false;
            }
        }
    }
}
