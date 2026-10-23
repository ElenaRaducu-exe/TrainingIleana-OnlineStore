using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class OrdersDashboard : ComponentBase
    {
        [Inject]
        private IApiClient _apiClient { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        [Inject]
        private AuthenticationStateProvider _authStateProvider { get; set; }

        private List<OrderDetailsModel> ordersList { get; set; } = new();

        private bool _isAdminUser { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var httpClient = await _apiClient.GetHttpClientAsync();

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _isAdminUser = user.IsInRole("Admin");

            if (_isAdminUser)
            {
                var ordersDTO = await httpClient.GetFromJsonAsync<List<OrderDetailsDTO>>("api/order/get/list");

                if (ordersDTO != null)
                {
                    ordersList = _mapper.Map<List<OrderDetailsModel>>(ordersDTO);
                }
            }
            else
            {
                var ordersDTO = await httpClient.GetFromJsonAsync<List<OrderDetailsDTO>>("api/order/get/list/customer");

                if (ordersDTO != null)
                {
                    ordersList = _mapper.Map<List<OrderDetailsModel>>(ordersDTO);
                }
            }
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
