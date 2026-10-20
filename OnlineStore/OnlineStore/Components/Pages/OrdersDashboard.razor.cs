using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class OrdersDashboard : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        [Inject]
        private AuthenticationStateProvider _authStateProvider { get; set; }

        private List<OrderDetailsModel> ordersList { get; set; } = new();

        private bool _isAdminUser { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _isAdminUser = user.IsInRole("Admin");

            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
