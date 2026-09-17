using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class CartProductsService : ICartProductsService
    {
        private List<ProductDTO> _cartProducts = new List<ProductDTO>();
        private readonly OnlineStoreContext _dbContext;

        public CartProductsService(OnlineStoreContext onlineStoreContext)
        {
            _dbContext = onlineStoreContext;
        }

        public async Task AddProductToCartProductsList(ProductDTO productDTO)
        {
            if (productDTO != null)
            {
                _cartProducts.Add(productDTO);
            }
        }

        public async Task<List<ProductDTO>?> GetCartProducts()
        {
            return _cartProducts;
        }

        public async Task AddProductToCart(int productId, int userId)
        {
            var cartItem = new CartItem()
            {
                Quantity = 1,
                ProductId = productId
            };

            await _dbContext.CartItems.AddAsync(cartItem);

            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"CartItem Id 2: {cartItem.Id}");

            var cart = new Cart()
            {
                UserId = userId,
                CartItemId = cartItem.Id
            };

            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }
    }
}