using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Services.Contracts
{
    public interface ICartProductsService
    {
        Task AddProductToCartProductsList(ProductDTO productDTO);

        Task<List<CartItemSummaries>?> GetCartItems();

        Task AddProductToCart(int productId, int userId);

        Task<List<CartItemDTO>?> GetCartItemsByUser(int userId);
    }
}