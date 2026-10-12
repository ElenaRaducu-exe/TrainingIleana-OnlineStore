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
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;

        public CartProductsService(OnlineStoreContext onlineStoreContext, 
                                   IMapper mapper)
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

        public async Task<bool> AddProductToCart(int productId, int userId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if(product == null || product.ReservedStock >= product.Stock)
            {
                return false; 
            }

            var existingCart = _dbContext.Carts.FirstOrDefault(c => c.UserId == userId);

            if(existingCart != null)
            {
                var existingCartItem = _dbContext.CartItems.FirstOrDefault(c => c.ProductId == productId && c.CartId == existingCart.Id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++;
                }
                else
                {
                    var cartItem = new CartItem()
                    {
                        Quantity = 1,
                        ProductId = productId,
                        CartId = existingCart.Id
                    };

                    await _dbContext.CartItems.AddAsync(cartItem);
                }
            }
            else
            {
                var cart = new Cart()
                {
                    UserId = userId
                };

                await _dbContext.Carts.AddAsync(cart);

                var cartItem = new CartItem()
                {
                    Quantity = 1,
                    ProductId = productId,
                    Cart = cart
                };

                await _dbContext.CartItems.AddAsync(cartItem);
            }

            product.ReservedStock++;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            if(newQuantity < 1)
            {
                return false;
            }

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

            int quantityDifference = newQuantity - initialQuantity;
            int newReservedStock = product.ReservedStock + quantityDifference;

            if(newReservedStock < 0 || newReservedStock > product.Stock)
            {
                return false; 
            }
     
            product.ReservedStock = newReservedStock;
            product.IsActive = product.ReservedStock < product.Stock;

            cartItem.Quantity = newQuantity;

            await _dbContext.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> DeleteCartItem(int cartItemId) 
        {
            var cartItem = _dbContext.CartItems.FirstOrDefault(item => item.Id == cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            var cart = _dbContext.Carts.FirstOrDefault(cart => cart.Id == cartItem.CartId);

            if (cart == null)
            {
                return false;
            }

            var product = _dbContext.Products.FirstOrDefault(product => product.Id == cartItem.ProductId);
            if(product == null)
            {
                return false; 
            }

            if(product.ReservedStock < cartItem.Quantity)
            {
                return false;
            }

            product.ReservedStock -= cartItem.Quantity;
            product.IsActive = product.ReservedStock < product.Stock;

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();

            if(!_dbContext.CartItems.Any(item => item.CartId == cart.Id))
            {
                _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
            }

            return true; 
        }
    }
}