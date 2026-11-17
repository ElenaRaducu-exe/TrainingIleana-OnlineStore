using Microsoft.AspNetCore.Components;
using MudBlazor;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class AdminOrdersPage : ComponentBase
    {
        [Inject]
        private IOrderFrontendService _orderFrontendService { get; set; }

        private MudDataGrid<OrderSummaryModel> _dataGrid { get; set; }

        private async Task<GridData<OrderSummaryModel>> LoadServerData(GridState<OrderSummaryModel> state, CancellationToken cancellationToken)
        {
            var orderFilters = new OrderFiltersModel()
            {
                PageNumber = state.Page,
                PageSize = state.PageSize
            };

            var pagedResult = await _orderFrontendService.GetOrdersPaged(orderFilters);

            return new GridData<OrderSummaryModel>()
            {
                Items = pagedResult.Items,
                TotalItems = pagedResult.TotalCount
            };
        }
    }
}


/*
_dataGrid.ReloadServerData(); 

_dataGrid.ServerItems = await _orderFrontendService.GetOrdersPaged(state.Page, state.PageSize); // lista de items

var x = _dataGrid.ServerItems.FirstOrDefault(i => i.OrderId == _currentOrderId);

var y = cancellationToken.*/