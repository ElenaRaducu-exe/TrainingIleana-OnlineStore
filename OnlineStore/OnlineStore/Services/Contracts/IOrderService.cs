namespace OnlineStore.Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> PlaceOrder(int addressId, int userId, int cartId); 
    }
}
