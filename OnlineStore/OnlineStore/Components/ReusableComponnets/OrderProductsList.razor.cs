using Microsoft.AspNetCore.Components;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class OrderProductsList : ComponentBase
    {
        [Inject]
        private IOrderFrontendService _orderFrontendService { get; set; }

        [Parameter]
        public int OrderId { get; set; }

        private List<OrderProductModel> _items { get; set; } = new();

        private bool _isLoading { get; set; }

        private int? _loadedOrderId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (_loadedOrderId != OrderId)
            {
                _loadedOrderId = OrderId;
                await LoadProducts();
            }
        }

        private async Task LoadProducts()
        {
            _isLoading = true;

            _items = await _orderFrontendService.GetOrderProductsByOrderId(OrderId);

            _isLoading = false;
        }
    }
}
