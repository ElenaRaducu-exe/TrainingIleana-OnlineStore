using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;
using System.Net.Http.Headers;

namespace OnlineStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;

        public OrderService(OnlineStoreContext dbContext,
                            IAddressService addressService,
                            IMapper mapper)
        {
            _dbContext = dbContext;
            _addressService = addressService;
            _mapper = mapper;
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
                    OrderNumber = "#ORD-" + createOrderRequestDTO.UserId + createOrderRequestDTO.AddressId + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                };

                await _dbContext.Orders.AddAsync(order);

                foreach (var cartItem in cartItems)
                {
                    var product = _dbContext.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);

                    if (product != null)
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

        public async Task<List<OrderDetailsDTO>?> GetOrdersDTO()
        {
            var orders = await _dbContext.Database.SqlQuery<OrderDetailsSP>($"exec dbo.GetOrders").ToListAsync();

            if (orders != null)
            {
                return _mapper.Map<List<OrderDetailsDTO>>(orders);
            }

            return null;
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            return order;
        }

        public async Task<List<OrderItemDetailsDTO>?> GetOrderItemsDTOListById(int orderId)
        {
            var orderItems = await _dbContext.Database.SqlQuery<OrderItemDetailsSP>($"exec dbo.GetOrderItemsDetails @OrderId={orderId}").ToListAsync();

            if (orderItems != null)
            {
                return _mapper.Map<List<OrderItemDetailsDTO>>(orderItems);
            }

            return null;
        }

        public async Task<OrderDetailsDTO?> GetOrderDetailsById(int orderId)
        {
            var orders = await _dbContext.Database.SqlQuery<OrderDetailsSP>($"exec dbo.GetOrders").ToListAsync();

            if (orders != null)
            {
                var orderById = orders.FirstOrDefault(o => o.OrderId == orderId);

                return _mapper.Map<OrderDetailsDTO>(orderById);
            }

            return null;
        }

        public async Task<OrderDetailsPageDTO?> GetOrderDetailsPageDTO(int orderId)
        {
            var orderDetails = await GetOrderDetailsById(orderId);

            if (orderDetails == null)
            {
                return null;
            }

            var address = await _addressService.GetAddressById(orderDetails.AddressId);

            if (address == null)
            {
                return null;
            }

            var orderItems = await GetOrderItemsDTOListById(orderId);

            if (orderItems == null)
            {
                return null;
            }

            return new OrderDetailsPageDTO()
            {
                OrderDetails = orderDetails,
                Address = address,
                OrderItems = orderItems
            };
        }

        public async Task<bool?> UpdateOrderState(int orderId, int newStatusId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false; 
            }

            if((await IsValidStatusTransiton(order.StatusId, newStatusId)) == false)
            {
                return null;
            }

            order.StatusId = newStatusId;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsValidStatusTransiton(int statusId, int newStatusId)
        {
            if((statusId == 1 && newStatusId == 2) || (statusId == 1 && newStatusId == 6))
            {
                return true; 
            }

            if((statusId == 2 && newStatusId == 3) || (statusId == 2 && newStatusId == 6))
            {
                return true; 
            }

            if((statusId == 3 && newStatusId == 8) || (statusId == 3 && newStatusId == 6))
            {
                return true; 
            }

            if(statusId == 8 && newStatusId == 4)
            {
                return true;
            }

            if(statusId == 4 && newStatusId == 5)
            {
                return true;
            }

            if(statusId == 5 && newStatusId == 7)
            {
                return true;
            }

            return false; 
        }
    }
}
