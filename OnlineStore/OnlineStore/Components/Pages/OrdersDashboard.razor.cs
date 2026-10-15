using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

using OnlineStore.Models.DTOs;
using MudBlazor;

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

        private List<OrderDetailsModel> ordersList { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var ordersDTO = await httpClient.GetFromJsonAsync<List<OrderDetailsDTO>>("api/order/get/list");

            if (ordersDTO != null)
            {
                ordersList = _mapper.Map<List<OrderDetailsModel>>(ordersDTO);
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
