using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface ICartProductsService
    {
        Task AddProductToCartProductsList(ProductDTO productDTO);

        Task<List<ProductDTO>?> GetCartProducts();

        Task AddProductToCart(int productId, int userId);
    }
}