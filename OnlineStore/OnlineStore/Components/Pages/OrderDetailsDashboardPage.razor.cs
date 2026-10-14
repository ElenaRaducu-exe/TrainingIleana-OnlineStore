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

        private OrderModel? _orderModel { get; set; }
        private AddressModel? _addressModel { get; set; }
        private List<OrderItemDetailsModel> _orderItemsList { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var orderItemsDTO = await httpClient.GetFromJsonAsync<List<OrderItemDetailsDTO>>($"api/order/get/items/{OrderId}");

            if(orderItemsDTO != null)
            {
                _orderItemsList = _mapper.Map<List<OrderItemDetailsModel>>(orderItemsDTO);

                int addressId = _orderItemsList.First().AddressId;

                var addressDTO = await httpClient.GetFromJsonAsync<AddressDTO>($"api/address/get/{addressId}");

                if (addressDTO != null)
                {
                    _addressModel = _mapper.Map<AddressModel>(addressDTO);
                }
            }

            var orderDetailsDTO = await httpClient.GetFromJsonAsync<OrderDetailsDTO>($"api/order/get/details/{OrderId}");

            if (orderDetailsDTO != null)
            {
                _orderModel = _mapper.Map<OrderModel>(orderDetailsDTO);
            }
        }
    }
}
