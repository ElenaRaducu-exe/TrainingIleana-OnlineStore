using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO);

        Task<List<OrderDetailsDTO>?> GetOrdersDTO();

        Task<Order?> GetOrderById(int orderId);

        Task<List<OrderItemDetailsDTO>?> GetOrderItemsDTOListById(int orderId);

        Task<OrderDetailsDTO?> GetOrderDetailsById(int orderId);

        Task<OrderDetailsPageDTO?> GetOrderDetailsPageDTO(int orderId); 
    }
}
