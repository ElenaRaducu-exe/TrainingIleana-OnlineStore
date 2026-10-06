using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly ICartProductsService _cartProductsService;

        public OrderService(OnlineStoreContext dbContext,
                            ICartProductsService cartProductsService)
        {
            _dbContext = dbContext;
            _cartProductsService = cartProductsService;
        }

        public async Task<bool> PlaceOrder(int addressId, int userId, int cartId)
        {
            var order = new Order()
            {
                AddressId = addressId,
                UserId = userId,
                StatusId = 1, 
                OrderNumber = "Order_" + userId + "_" + DateTime.UtcNow.ToString("yyyy.MM.dd_HH:mm:ss")
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            var cartItems = await _dbContext.CartItems.Where(item => item.CartId  == cartId).ToListAsync();

            if(cartItems != null)
            {
                foreach(var cartItem in cartItems)
                {
                    var orderItem = new OrderItem()
                    {
                        OrderId = order.Id,
                        Quantity = cartItem.Quantity,
                        ProductId = cartItem.ProductId,
                        UnitPrice = _dbContext.Products.FirstOrDefault(p => p.Id == cartItem.ProductId).Price
                    };

                    await _dbContext.OrderItems.AddAsync(orderItem);
                    await _dbContext.SaveChangesAsync();
                }

                await _cartProductsService.DeleteCart(userId);

                return true;
            }

            return false;
        }
    }
}
