using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class OrdersDashboard : ComponentBase
    {
        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private AuthenticationStateProvider _authStateProvider { get; set; }

        [Inject]
        private IOrderFrontendService _orderFrontendService { get; set; }

        private List<OrderDetailsModel> ordersList { get; set; } = new();

        private bool _isAdminUser { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _isAdminUser = user.IsInRole("Admin");

            ordersList = await _orderFrontendService.GetOrderDetailsModelList(_isAdminUser);
        }

        private void OrderRowClicked(TableRowClickEventArgs<OrderDetailsModel> orderClicked)
        {
            if (orderClicked.Item != null)
            {
                _navigation.NavigateTo($"/order-details-dashboard/{orderClicked.Item.OrderId}");
            }
        }

    }
}
