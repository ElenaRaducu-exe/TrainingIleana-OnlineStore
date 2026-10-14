using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO);

        Task<List<OrderDetailsDTO>?> GetOrdersDTO();
    }
}
