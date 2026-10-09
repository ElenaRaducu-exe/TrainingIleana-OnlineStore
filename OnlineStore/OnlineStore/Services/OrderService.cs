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
        private readonly IProductService _productService;

        public OrderService(OnlineStoreContext dbContext,
                            ICartProductsService cartProductsService,
                            IProductService productService)
        {
            _dbContext = dbContext;
            _cartProductsService = cartProductsService;
            _productService = productService;
        }

        public async Task<bool> PlaceOrder(CreateOrderRequestDTO createOrderRequestDTO)
        {
            var cartItems = await _dbContext.CartItems.Where(item => item.CartId == createOrderRequestDTO.CartId).ToListAsync();

            if (cartItems != null)
            {
                var order = new Order()
                {
                    AddressId = createOrderRequestDTO.AddressId,
                    UserId = createOrderRequestDTO.UserId,
                    StatusId = 1,
                    OrderNumber = "Order_" + createOrderRequestDTO.UserId + "_" + DateTime.UtcNow.ToString("yyyy.MM.dd_HHmmss")
                };

                await _dbContext.Orders.AddAsync(order);

                foreach (var cartItem in cartItems)
                {
                    var product = _dbContext.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);

                    if(product != null)
                    {
                        var orderItem = new OrderItem()
                        {
                            Order = order,
                            Quantity = cartItem.Quantity,
                            ProductId = cartItem.ProductId,
                            UnitPrice = product.Price
                        };

                        await _dbContext.OrderItems.AddAsync(orderItem);

                        product.Stock -= cartItem.Quantity;

                        product.ReservedStock -= cartItem.Quantity;
                    }
                }

                var cart = await _dbContext.Carts.FirstOrDefaultAsync(cart => cart.UserId == createOrderRequestDTO.UserId);

                if (cart == null)
                {
                    return false;
                }

                _dbContext.CartItems.RemoveRange(cartItems);

                _dbContext.Carts.Remove(cart);

                await _dbContext.SaveChangesAsync();
            }

            return false;
        }
    }
}
