using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Services.Backend.Contracts
{
    public interface ICartProductsService
    {
        Task<List<CartItemSummaries>?> GetCartItems();

        Task<bool?> AddProductToCart(int productId, int userId);

        Task<CartSummaryPageModelDto?> GetCartItemsByUser(int userId);

        Task<bool> UpdateCartItemQuantity(int cartItem, int quantity);

        Task<bool> DeleteCartItem(int cartItemId);

        Task<bool> DeleteCart(int cartId);
    }
}