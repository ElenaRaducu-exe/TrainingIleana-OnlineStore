using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using System.Net.Http.Headers;

namespace OnlineStore.Components.Pages
{
    public partial class OrderDetailsDashboardPage : ComponentBase
    {
        [Parameter]
        public int OrderId { get; set; }

        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Inject]
        private ITokenProvider _tokenProvider { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private OrderDetailsPageModel? _orderDetailsPageModel { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // api/order/get/details-page/{orderId:int}

            var orderDetailsPageDTO = await httpClient.GetFromJsonAsync<OrderDetailsPageDTO>($"api/order/get/details-page/{OrderId}"); 

            if(orderDetailsPageDTO != null)
            {
                _orderDetailsPageModel = _mapper.Map<OrderDetailsPageModel>(orderDetailsPageDTO); 
            }
        }
    }
}
