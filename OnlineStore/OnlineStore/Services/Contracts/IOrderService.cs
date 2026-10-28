using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO);

        Task<Order?> GetOrderById(int orderId);

        Task<List<OrderItemDetailsDTO>?> GetOrderItemsDTOListById(int orderId);

        Task<OrderDetailsDTO?> GetOrderDetailsById(int orderId);

        //Task<OrderDetailsPageDTO?> GetOrderDetailsPageDTO(int orderId);

        Task<bool?> UpdateOrderStatus(int orderId, int newStatusId, int userId);

        Task<bool> UpdateProductsStockAfterCancelledOrder(int orderId);

        Task<OrderDetailsPageDTO?> GetOrderSummaryRaport(int orderId);

        Task<List<OrderDetailsDTO>?> GetOrderDetailsDtos(int? customerId);
    }
}
