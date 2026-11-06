using Microsoft.AspNetCore.Components;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class CartItemCard : ComponentBase
    {
        [Inject]
        private ICartFrontendService _cartFrontendService { get; set; }

        [Inject]
        private IProductFrontendService _productFrontendService { get; set; }

        [Parameter]
        public CartItemModel CartItemModel { get; set; } 

        [Parameter]
        public EventCallback OnQuantityUpdated { get; set; }

        [Parameter]
        public EventCallback<int> OnItemSelected { get; set; }

        [Parameter]
        public EventCallback<int> OnItemDeleted { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public decimal TotalPrice { get; set; }

        private int _quantity { get; set; }

        private ProductModel _product { get; set; } = new ProductModel();

        private bool _disabledIncreaseQuantityBtn = false;

        protected override async Task OnInitializedAsync()
        {
            TotalPrice = CartItemModel.Price * CartItemModel.Quantity;
            _quantity = CartItemModel.Quantity;

            var resultProduct = await _productFrontendService.GetProductModelById(CartItemModel.ProductId); 

            if( resultProduct != null)
            {
                _product = resultProduct;
            }

            if(_product.ReservedStock == _product.Stock)
            {
                _disabledIncreaseQuantityBtn = true; 
            }
        }

        private async Task IncreaseQuantity()
        {   
            int increasedQuantity = _quantity + 1;

            if (_product.ReservedStock >= _product.Stock)
            {
                _disabledIncreaseQuantityBtn = true;
                return;
            }

            var responseUpdateQuantity = await _cartFrontendService.UpdateCartItemQuantity(CartItemModel.CartItemId, increasedQuantity);

            if (responseUpdateQuantity == true)
            {
                _quantity = increasedQuantity;

                CartItemModel.Quantity = _quantity;

                TotalPrice = CartItemModel.Price * _quantity;

                _disabledIncreaseQuantityBtn = _product.ReservedStock >= _product.Stock;

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

            var responseUpdateQuantity = await _cartFrontendService.UpdateCartItemQuantity(CartItemModel.CartItemId, decreasedQuantity);

            if (responseUpdateQuantity == true)
            {
                _quantity = decreasedQuantity;

                CartItemModel.Quantity = _quantity;

                TotalPrice = CartItemModel.Price * _quantity;

                _disabledIncreaseQuantityBtn = _product.ReservedStock >= _product.Stock;

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
