using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;
using static MudBlazor.CategoryTypes;

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
            var existingCart = _dbContext.Carts.FirstOrDefault(c => c.UserId == userId);

            if(existingCart != null)
            {
                var existingCartItem = _dbContext.CartItems.FirstOrDefault(c => c.ProductId == productId && c.CartId == existingCart.Id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++;
                    await _dbContext.SaveChangesAsync();
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

                    await _dbContext.SaveChangesAsync();

                    return;
                }
            }
            else
            {
                var cart = new Cart()
                {
                    UserId = userId
                };

                await _dbContext.Carts.AddAsync(cart);
                await _dbContext.SaveChangesAsync();

                var cartItem = new CartItem()
                {
                    Quantity = 1,
                    ProductId = productId,
                    CartId = cart.Id
                };

                await _dbContext.CartItems.AddAsync(cartItem);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateCartItemQuantity(int cartItemId, int newQuantity)
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

            var newProductStock = product.Stock - newQuantity + initialQuantity;

            if(newProductStock < 0)
            {
                return false; 
            }
     
            product.Stock = newProductStock;
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
            product.Stock += cartItem.Quantity;

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();

            if(_dbContext.CartItems.Count() == 0)
            {
                _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
            }

            return true; 
        }
    }
}