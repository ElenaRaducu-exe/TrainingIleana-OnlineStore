using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface IOrderFrontendService
    {
        Task<OrderDetailsPageModel?> GetOrderDetailsPageModel(int orderId);
        Task<bool> UpdateStock(int orderId, OrderDetailsPageModel orderDetailsPageModel); 
        Task<OrderDetailsPageModel?> UpdateOrderStatus(int orderId, int newStatusId, OrderDetailsPageModel orderDetailsPageModel);
        Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO);
        Task<List<OrderDetailsModel>> GetOrderDetailsModelList(bool isAdminUser); 
    }
}
