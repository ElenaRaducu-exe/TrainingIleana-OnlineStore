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
            var cartItemSummeris = await _dbContext.Database.SqlQuery<CartItemSummaries>($"exec dbo.spGetCartItemsDetailsUser @UserId={userId}").ToListAsync();

            var cartItemDTOs = _mapper.Map<List<CartItemDTO>>(cartItemSummeris);

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
    }
}