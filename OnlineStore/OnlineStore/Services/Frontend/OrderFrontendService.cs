using AutoMapper;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.Enums;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Services.Frontend
{
    public class OrderFrontendService : IOrderFrontendService
    {
        private readonly IApiClient _apiClient;
        private readonly IMapper _mapper;

        public OrderFrontendService(IApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<OrderDetailsPageModel?> GetOrderDetailsPageModel(int orderId)
        {
            var orderSummaryRaport = await _apiClient.GetAsync<OrderDetailsPageDTO>($"api/order/get/details-page/summary-raport/{orderId}");

            if(orderSummaryRaport != null)
            {
                try
                {
                    var orderDetailsPageModel = _mapper.Map<OrderDetailsPageModel>(orderSummaryRaport);
                    return orderDetailsPageModel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"---------------------------------------------------------- Error with mapping in OrderFrontendService: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        public async Task<bool> UpdateStock(int orderId, OrderDetailsPageModel orderDetailsPageModel)
        {
            var responseRestockProducts = await _apiClient.PutAsJsonAsync($"api/order/update/stock/cancelled-order/{orderId}", orderDetailsPageModel);

            if(responseRestockProducts.IsSuccessStatusCode)
            {
                return true;
            }

            return false; 
        }

        public async Task<OrderDetailsPageModel?> UpdateOrderStatus(int orderId, int newStatusId, OrderDetailsPageModel orderDetailsPageModel)
        {
            var responseChangeStatus = await _apiClient.PutAsJsonAsync($"api/order/update/status/{orderId}/{newStatusId}", orderDetailsPageModel);

            if (orderDetailsPageModel != null && orderDetailsPageModel.OrderDetails != null)
            {
                if (responseChangeStatus.IsSuccessStatusCode)
                {
                    if(newStatusId == (int)OrderStatusEnum.Cancelled)
                    {
                        var responseRestockProducts = await UpdateStock(orderId, orderDetailsPageModel);

                        if(responseRestockProducts == true)
                        {
                            orderDetailsPageModel.OrderDetails.StatusEnum = (OrderStatusEnum)newStatusId;
                            orderDetailsPageModel.OrderDetails.Status = orderDetailsPageModel.OrderDetails.StatusEnum.ToString();
                            orderDetailsPageModel.OrderDetails.ModifiedAt = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        orderDetailsPageModel.OrderDetails.StatusEnum = (OrderStatusEnum)newStatusId;
                        orderDetailsPageModel.OrderDetails.Status = orderDetailsPageModel.OrderDetails.StatusEnum.ToString();
                        orderDetailsPageModel.OrderDetails.ModifiedAt = DateTime.UtcNow;
                    }
                }

                return orderDetailsPageModel;
            }
            return null;
        }

        public async Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO)
        {
            if(createOrderRequestDTO.AddressId != null)
            {
                var result = await _apiClient.PostAsJsonAsync($"api/order/create", createOrderRequestDTO);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<OrderDetailsModel>> GetOrderDetailsModelList(bool isAdminUser)
        {
            if (isAdminUser)
            {
                var ordersDTO = await _apiClient.GetAsync<List<OrderDetailsDTO>>("api/order/get/list");

                if (ordersDTO != null)
                {
                    return _mapper.Map<List<OrderDetailsModel>>(ordersDTO);
                }
            }
            else
            {
                var ordersDTO = await _apiClient.GetAsync<List<OrderDetailsDTO>>("api/order/get/list/customer");

                if (ordersDTO != null)
                {
                    return _mapper.Map<List<OrderDetailsModel>>(ordersDTO);
                }
            }

            return new List<OrderDetailsModel>();
        }
    }
}
