using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface ICartFrontendService
    {
        Task<ProductModel?> AddProductToCart(ProductModel productModel);
        Task<bool> DeleteCartItem(int cartItemId);
        Task<bool> DeleteCart(int cartId);
        Task<List<CartItemModel>> GetCartItemsByUser();
        Task<bool> UpdateCartItemQuantity(int cartItemId, int newQuantity); 
    }
}
