using Microsoft.AspNetCore.Components;
using OnlineStore.Models.Enums;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.Pages
{
    public partial class OrderDetailsDashboardPage : ComponentBase
    {
        [Parameter]
        public int OrderId { get; set; }

        [Inject]
        private IOrderFrontendService _orderFrontendService { get; set; }

        private OrderDetailsPageModel? _orderDetailsPageModel { get; set; } = new();

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

        private bool _disableReturnRequestButtonAfterDeliverDate = false;
        private DateTime? _expirationDateReturnRequest = null; 

        protected override async Task OnInitializedAsync()
        {
            _orderDetailsPageModel = await _orderFrontendService.GetOrderDetailsPageModel(OrderId);

            _expirationDateReturnRequest = _orderDetailsPageModel?.OrderDetails?.ModifiedAt.AddDays(14);
            if (DateTime.UtcNow > _expirationDateReturnRequest)
            {
                _disableReturnRequestButtonAfterDeliverDate = true;
            }
        }

        public async Task ChangeStatus(int orderId, int newStatusId)
        {
            if(_orderDetailsPageModel != null)
            {
                _orderDetailsPageModel = await _orderFrontendService.UpdateOrderStatus(orderId, newStatusId, _orderDetailsPageModel);

                _disableReturnRequestButtonAfterDeliverDate = false;

                StateHasChanged();
            }
        }
    }
}
