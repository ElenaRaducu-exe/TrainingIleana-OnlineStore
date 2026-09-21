using AutoMapper;
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
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;

        public CartProductsService(OnlineStoreContext onlineStoreContext, IMapper mapper)
        {
            _dbContext = onlineStoreContext;
            _mapper = mapper;
        }

        public async Task<List<CartItemSummaries>?> GetCartItems()
        {
            return await _dbContext.Database.SqlQuery<CartItemSummaries>($"exec dbo.spGetCartItemsDetails").ToListAsync();
        }

        public async Task<List<CartItemDTO>?> GetCartItemsByUser(int userId)
        {
            var cartItemSummaries = await _dbContext.Database.SqlQuery<CartItemSummaries>($"exec dbo.spGetCartItemsDetailsUser @UserId={userId}").ToListAsync();

            var cartItemDTOs = _mapper.Map<List<CartItemDTO>>(cartItemSummaries);

            return cartItemDTOs;
        }

        public async Task AddProductToCartProductsList(ProductDTO productDTO)
        {
            if (productDTO != null)
            {
                _cartProducts.Add(productDTO);
            }
        }

        public async Task AddProductToCart(int productId, int userId)
        {
            var existingCart = _dbContext.Carts.Include(c => c.CartItem).FirstOrDefault(c => 
                        c.UserId == userId && c.CartItem != null && c.CartItem.ProductId == productId);

            if(existingCart != null)
            {
                existingCart.CartItem.Quantity++;

                await _dbContext.SaveChangesAsync();

                return;
            }

            var cartItem = new CartItem()
            {
                Quantity = 1,
                ProductId = productId
            };

            await _dbContext.CartItems.AddAsync(cartItem);

            await _dbContext.SaveChangesAsync();

            var cart = new Cart()
            {
                UserId = userId,
                CartItemId = cartItem.Id
            };

            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            /*
            var cartItemSummary = await _dbContext.Database.SqlQuery<CartItemSummaries>
                ($"exec [dbo].[spGetCartItem] @UserId = {userId}, @CartItemId = {cartItemId}").FirstOrDefaultAsync();

            if( cartItemSummary == null )
            {
                return false; 
            }*/

            var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync(item => item.Id == cartItemId);

            if(cartItem  == null )
            {
                return false; 
            }

            int initialQuantity = cartItem.Quantity;

            var product = await _dbContext.Products.FirstOrDefaultAsync(product => product.Id == cartItem.ProductId);

            if( product == null )
            {
                return false; 
            }

            var newProductStock = product.Stock - quantity + initialQuantity;

            if(newProductStock < 0)
            {
                return false; 
            }

            product.Stock = newProductStock;
            cartItem.Quantity = quantity;

            await _dbContext.SaveChangesAsync();

            return true; 
        }
    }
}