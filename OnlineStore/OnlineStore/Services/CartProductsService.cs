using Microsoft.EntityFrameworkCore;
using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class CartProductsService : ICartProductsService
    {
        private List<ProductDTO> _cartProducts = new List<ProductDTO>();
        private List<CartItemSummaries> _cartItems = new List<CartItemSummaries>();
        private readonly OnlineStoreContext _dbContext;

        public CartProductsService(OnlineStoreContext onlineStoreContext)
        {
            _dbContext = onlineStoreContext;
        }

        public async Task GetCartItems()
        {
            _cartItems = await _dbContext.Database.SqlQuery<CartItemSummaries>($"exec dbo.spGetCartItemsDetails").ToListAsync();
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
            if (_dbContext.CartItems.Any(p => p.ProductId == productId))
            {
                var cartItemExisted = _dbContext.CartItems.FirstOrDefault(p => p.ProductId == productId);
                if(cartItemExisted != null)
                {
                    cartItemExisted.Quantity++;
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
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
}