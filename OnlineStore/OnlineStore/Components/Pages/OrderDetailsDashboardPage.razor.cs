using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.Enums;
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

        private string OrderActionButtonText => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Pending => Res.Lang.Btn_ConfirmOrder, 
            OrderStatusEnum.Confirmed => Res.Lang.Btn_StartProcessing,
            OrderStatusEnum.Processing => Res.Lang.Btn_CompleteProcessing,
            OrderStatusEnum.Prepared => Res.Lang.Btn_ConfirmCourierPickup,
            OrderStatusEnum.PickedUp => Res.Lang.Btn_ConfirmShipment,
            OrderStatusEnum.Shipped => Res.Lang.Btn_StartDelivering,
            _ => string.Empty
        };

        private int OrderNewStatusId => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Pending => (int)OrderStatusEnum.Confirmed,
            OrderStatusEnum.Confirmed => (int)OrderStatusEnum.Processing,
            OrderStatusEnum.Processing => (int)OrderStatusEnum.Prepared,
            OrderStatusEnum.Prepared => (int)OrderStatusEnum.PickedUp,
            OrderStatusEnum.PickedUp => (int)OrderStatusEnum.Shipped,
            OrderStatusEnum.Shipped => (int)OrderStatusEnum.Delivered,
            _ => 0
        };

        protected override async Task OnInitializedAsync()
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var orderDetailsPageDTO = await httpClient.GetFromJsonAsync<OrderDetailsPageDTO>($"api/order/get/details-page/{OrderId}"); 

            if(orderDetailsPageDTO != null)
            {
                _orderDetailsPageModel = _mapper.Map<OrderDetailsPageModel>(orderDetailsPageDTO); 
            }
        }

        public async Task ChangeStatus(int orderId, int newStatusId)
        {
            var token = await _tokenProvider.GetToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var responseChangeStatus = await httpClient.PutAsJsonAsync($"api/order/update/status/{orderId}/{newStatusId}", _orderDetailsPageModel);

            if(_orderDetailsPageModel != null && _orderDetailsPageModel.OrderDetails != null)
            {
                if (responseChangeStatus.IsSuccessStatusCode)
                {
                    if (newStatusId == (int)OrderStatusEnum.Cancelled)
                    {
                        var responseRestockProducts = await httpClient.PutAsJsonAsync($"api/order/update/stock/cancelled-order/{orderId}", _orderDetailsPageModel);

                        if (responseRestockProducts.IsSuccessStatusCode)
                        {
                            _orderDetailsPageModel.OrderDetails.StatusEnum = (OrderStatusEnum)newStatusId;
                            _orderDetailsPageModel.OrderDetails.Status = _orderDetailsPageModel.OrderDetails.StatusEnum.ToString();
                        }
                    }

                    _orderDetailsPageModel.OrderDetails.StatusEnum = (OrderStatusEnum)newStatusId;
                    _orderDetailsPageModel.OrderDetails.Status = _orderDetailsPageModel.OrderDetails.StatusEnum.ToString();

                    StateHasChanged();
                }
            }
        }
    }
}
