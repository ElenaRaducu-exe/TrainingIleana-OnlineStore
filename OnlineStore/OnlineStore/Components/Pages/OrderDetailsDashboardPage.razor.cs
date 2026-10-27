using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.Enums;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.Pages
{
    public partial class OrderDetailsDashboardPage : ComponentBase
    {
        [Parameter]
        public int OrderId { get; set; }

        [Inject]
        private IApiClient _apiClient { get; set; }

        [Inject]
        private IMapper _mapper { get; set; }

        private OrderDetailsPageModel? _orderDetailsPageModel { get; set; } = new();

        private bool _customerReturnRequestButton = false;
        private bool _ShowRejectedReturnButton = false;

        private string _orderActionButtonText => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Pending => Res.Lang.Btn_ConfirmOrder, 
            OrderStatusEnum.Confirmed => Res.Lang.Btn_StartProcessing,
            OrderStatusEnum.Processing => Res.Lang.Btn_CompleteProcessing,
            OrderStatusEnum.Prepared => Res.Lang.Btn_ConfirmCourierPickup,
            OrderStatusEnum.PickedUp => Res.Lang.Btn_ConfirmShipment,
            OrderStatusEnum.Shipped => Res.Lang.Btn_StartDelivering,

            // start return flow 
            OrderStatusEnum.ReturnedRequested => Res.Lang.Btn_ConfirmCourierPickup, 
            OrderStatusEnum.ReturnPickedUp => Res.Lang.Btn_ReturnInTransit, 
            OrderStatusEnum.ReturnInTransit => Res.Lang.Btn_ReturnUnderInspection, 
            OrderStatusEnum.ReturnUnderInspection => Res.Lang.Btn_ReturnApproved, 
            OrderStatusEnum.ReturnApproved => Res.Lang.Btn_ReturnProcessing, 
            OrderStatusEnum.ReturnProcessing => Res.Lang.Btn_Returned,
            OrderStatusEnum.ReturnRejected => Res.Lang.Btn_ReturnShippingBack, 
            OrderStatusEnum.ReturnShippingBack => Res.Lang.Btn_ReturnDelivered, 
            OrderStatusEnum.ReturnDelivered => Res.Lang.Btn_ReturnClose,
            _ => string.Empty
        };

        private int _orderNewStatusId => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Pending => (int)OrderStatusEnum.Confirmed,
            OrderStatusEnum.Confirmed => (int)OrderStatusEnum.Processing,
            OrderStatusEnum.Processing => (int)OrderStatusEnum.Prepared,
            OrderStatusEnum.Prepared => (int)OrderStatusEnum.PickedUp,
            OrderStatusEnum.PickedUp => (int)OrderStatusEnum.Shipped,
            OrderStatusEnum.Shipped => (int)OrderStatusEnum.Delivered,

            // start return flow 
            OrderStatusEnum.ReturnedRequested => (int)OrderStatusEnum.ReturnPickedUp,
            OrderStatusEnum.ReturnPickedUp => (int)OrderStatusEnum.ReturnInTransit,
            OrderStatusEnum.ReturnInTransit => (int)OrderStatusEnum.ReturnUnderInspection,
            OrderStatusEnum.ReturnUnderInspection => (int)OrderStatusEnum.ReturnApproved,
            OrderStatusEnum.ReturnApproved => (int)OrderStatusEnum.ReturnProcessing,
            OrderStatusEnum.ReturnProcessing => (int)OrderStatusEnum.Returned,
            OrderStatusEnum.ReturnRejected => (int)OrderStatusEnum.ReturnShippingBack,
            OrderStatusEnum.ReturnShippingBack => (int)OrderStatusEnum.ReturnDelivered,
            OrderStatusEnum.ReturnDelivered => (int)OrderStatusEnum.ReturnClosed,
            _ => 0
        };

        private string _orderActionButtonTextRequestedReturn => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Delivered => Res.Lang.Btn_ReturnRequest, 
            _ => string.Empty
        };

        private int _orderNewStatusIdRequestedReturn => _orderDetailsPageModel?.OrderDetails?.StatusEnum switch
        {
            OrderStatusEnum.Delivered => (int)OrderStatusEnum.ReturnedRequested,
            _ => 0
        };

        protected override async Task OnInitializedAsync()
        {
            var httpClient = await _apiClient.GetHttpClientAsync();

            var orderDetailsPageDTO = await httpClient.GetFromJsonAsync<OrderDetailsPageDTO>($"api/order/get/details-page/{OrderId}"); 

            if(orderDetailsPageDTO != null)
            {
                _orderDetailsPageModel = _mapper.Map<OrderDetailsPageModel>(orderDetailsPageDTO); 
            }
        }

        public async Task ChangeStatus(int orderId, int newStatusId)
        {
            var httpClient = await _apiClient.GetHttpClientAsync();

            // date !!!!! -- orderDetailsPageDTO.OrderDetails.modifiedAt trebuie modificat => in service, nu aici 

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
