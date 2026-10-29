using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models.Configuration;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.Services.Contracts;
using System.Data;
using System.Net.Http.Headers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OnlineStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;

        private readonly DbConfiguration _dbConfiguration;

        public OrderService(OnlineStoreContext dbContext,
                            IMapper mapper,
                            DbConfiguration dbConfiguration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dbConfiguration = dbConfiguration;
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

        public async Task<List<OrderDetailsDTO>?> GetOrderDetailsDtos(int? customerId)
        {
            try
            {
                var resultStoredProcedure = new List<OrderDetailsSP>();

                DataSet ds = new DataSet();

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("dbo.spGetOrders", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        if (customerId != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            var order = new OrderDetailsSP()
                            {
                                OrderId = (int)row["OrderId"],
                                StatusId = (int)row["StatusId"],
                                UserId = (int)row["UserId"],
                                AddressId = (int)row["AddressId"],
                                OrderNumber = row["OrderNumber"].ToString() ?? "-",
                                OrderDate = (DateTime)row["OrderDate"],
                                ProductsNumber = (int)row["ProductsNumber"],
                                TotalQuantityItems = (int)row["TotalQuantityItems"],
                                TotalPrice = (decimal)row["TotalPrice"],
                                Username = row["Username"].ToString() ?? "-",
                                FullName = row["FullName"].ToString() ?? "-",
                                Email = row["Email"].ToString() ?? "-",
                                ModifiedAt = (DateTime)row["ModifiedAt"]
                            };

                            resultStoredProcedure.Add(order);
                        }

                        return _mapper.Map<List<OrderDetailsDTO>>(resultStoredProcedure);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in spGetOrderSummaryRaport: {ex.Message}");
                return null;
            }
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
            try 
            { 
                var resultStoredProcedure = new List<OrderItemDetailsSP>();

                DataSet dataSet = new DataSet();

                using(SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("spGetOrderItemsDetails", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCommand.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            var orderItem = new OrderItemDetailsSP()
                            {
                                OrderItemId = (int)row["OrderItemId"],
                                ProductId = (int)row["ProductId"],
                                Quantity = (int)row["Quantity"],
                                UnitPrice = (decimal)row["UnitPrice"],
                                ProductName = row["ProductName"].ToString() ?? "-",
                                ProductDescription = row["ProductDescription"].ToString() ?? "-",
                                BrandName = row["BrandName"].ToString() ?? "-",
                                CategoryName = row["CategoryName"].ToString() ?? "-",
                                TotalPrice = (decimal)row["TotalPrice"]
                            };

                            resultStoredProcedure.Add(orderItem);
                        }

                        return _mapper.Map<List<OrderItemDetailsDTO>>(resultStoredProcedure);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in spGetOrderItemsDetails: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderDetailsDTO?> GetOrderDetailsById(int orderId)
        {
            try
            {
                var resultStoredProcedure = new OrderDetailsSP();

                DataSet dataSet = new DataSet();

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using(SqlCommand sqlCommand = new SqlCommand("spGetOrderDetailsbyId", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCommand.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            adapter.Fill(dataSet);
                        }

                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            resultStoredProcedure.OrderId = (int)row["OrderId"];
                            resultStoredProcedure.StatusId = (int)row["StatusId"];
                            resultStoredProcedure.UserId = (int)row["UserId"];
                            resultStoredProcedure.AddressId = (int)row["AddressId"];
                            resultStoredProcedure.OrderNumber = row["OrderNumber"].ToString() ?? "-";
                            resultStoredProcedure.OrderDate = (DateTime)row["OrderDate"];
                            resultStoredProcedure.ProductsNumber = (int)row["ProductsNumber"];
                            resultStoredProcedure.TotalQuantityItems = (int)row["TotalQuantityItems"];
                            resultStoredProcedure.TotalPrice = (decimal)row["TotalPrice"];
                            resultStoredProcedure.Username = row["Username"].ToString() ?? "-";
                            resultStoredProcedure.FullName = row["FullName"].ToString() ?? "-";
                            resultStoredProcedure.Email = row["Email"].ToString() ?? "-";
                            resultStoredProcedure.ModifiedAt = (DateTime)row["ModifiedAt"];
                        }

                        return _mapper.Map<OrderDetailsDTO>(resultStoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in dbo.spGetProducts: {ex.Message}");
                return null;
            }
        }

        /*
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
        */

        public async Task<bool?> UpdateOrderStatus(int orderId, int newStatusId, int userId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            if ((await IsValidStatusTransiton(order.StatusId, newStatusId)) == false)
            {
                return false;
            }

            order.StatusId = newStatusId;

            order.ModifiedAt = DateTime.UtcNow;
            order.ModifiedBy = userId;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsValidStatusTransiton(int statusId, int newStatusId)
        {
            // 1 = Pending => 2 = Confirmed
            if ((statusId == 1 && newStatusId == 2) || (statusId == 1 && newStatusId == 6))
            {
                return true;
            }

            // 2 = Confirmed => 3 = Processing
            if ((statusId == 2 && newStatusId == 3) || (statusId == 2 && newStatusId == 6))
            {
                return true;
            }

            // 3 = Processing => 9 = Prepared
            if ((statusId == 3 && newStatusId == 9) || (statusId == 3 && newStatusId == 6))
            {
                return true;
            }

            // 9 = Prepared => 8 = PickedUp
            if ((statusId == 9 && newStatusId == 8) || (statusId == 3 && newStatusId == 6))
            {
                return true;
            }

            // 8 = PickedUp => 4 = Shipped
            if (statusId == 8 && newStatusId == 4)
            {
                return true;
            }

            // 4 = Shipped => 5 = Delivered
            if (statusId == 4 && newStatusId == 5)
            {
                return true;
            }

            // 5 = Delivered => 7 = Returned, 10 = ReturnedRequested
            if ((statusId == 5 && newStatusId == 7) || (statusId == 5 && newStatusId == 10))
            {
                return true;
            }

            // 10 = ReturnedRequested => 11 = ReturnPickedUp
            if (statusId == 10 && newStatusId == 11)
            {
                return true;
            }

            // 11 = ReturnPickedUp => 12 = ReturnInTransit
            if (statusId == 11 && newStatusId == 12)
            {
                return true;
            }

            // 12 = ReturnInTransit => 13 = ReturnUnderInspection
            if (statusId == 12 && newStatusId == 13)
            {
                return true;
            }

            // 13 = ReturnUnderInspection => 14 = ReturnApproved, 16 = ReturnRejected
            if ((statusId == 13 && newStatusId == 14) || (statusId == 13 && newStatusId == 16))
            {
                return true;
            }

            // 14 = ReturnApproved => 15 = ReturnProcessing
            if (statusId == 14 && newStatusId == 15)
            {
                return true;
            }

            // 15 = ReturnProcessing => 7 = Returned
            if (statusId == 15 && newStatusId == 7)
            {
                return true;
            }

            // 16 = ReturnRejected => 17 = ReturnShippingBack
            if (statusId == 16 && newStatusId == 17)
            {
                return true;
            }

            // 17 = ReturnShippingBack => 18 = ReturnDelivered
            if (statusId == 17 && newStatusId == 18)
            {
                return true;
            }

            // 18 = ReturnDelivered => 19 = ReturnClosed
            if (statusId == 18 && newStatusId == 19)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateProductsStockAfterCancelledOrder(int orderId)
        {
            var orderItems = await _dbContext.OrderItems.Where(item => item.OrderId == orderId).ToListAsync();

            if (orderItems == null || !orderItems.Any())
            {
                return false;
            }

            foreach (var orderItem in orderItems)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);

                if (product != null)
                {
                    product.Stock += orderItem.Quantity;
                }
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<OrderDetailsPageDTO?> GetOrderSummaryRaport(int orderId)
        {
            try
            {
                var resultStoredProcedure = new OrderSummaryRaportSP();

                DataSet ds = new DataSet();

                using (SqlConnection connection = new SqlConnection(_dbConfiguration.OnlineStoreDb))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("spGetOrderSummaryRaport", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }

                        // order summary
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            // order information
                            resultStoredProcedure.AddressId = (int)row["AddressId"];
                            resultStoredProcedure.UserId = (int)row["UserId"];
                            resultStoredProcedure.OrderId = (int)row["OrderId"];
                            resultStoredProcedure.OrderNumber = row["OrderNumber"].ToString() ?? "-";
                            resultStoredProcedure.OrderDate = (DateTime)row["OrderDate"];
                            resultStoredProcedure.ProductsNumber = (int)row["ProductsNumber"];
                            resultStoredProcedure.TotalQuantityItems = (int)row["TotalQuantityItems"];
                            resultStoredProcedure.TotalPrice = (decimal)row["TotalPrice"];
                            resultStoredProcedure.StatusId = (int)row["StatusId"];
                            resultStoredProcedure.Username = row["Username"].ToString() ?? "-";
                            resultStoredProcedure.FullName = row["FullName"].ToString() ?? "-";
                            resultStoredProcedure.Email = row["Email"].ToString() ?? "-";
                            resultStoredProcedure.ModifiedAt = (DateTime)row["ModifiedAt"];

                            // address information
                            resultStoredProcedure.FirstNameAddress = row["FirstNameAddress"].ToString() ?? "-";
                            resultStoredProcedure.LastNameAddress = row["LastNameAddress"].ToString() ?? "-";
                            resultStoredProcedure.Phone = row["Phone"].ToString() ?? "-";
                            resultStoredProcedure.Street = row["Street"].ToString() ?? "-";
                            resultStoredProcedure.StreetNumber = row["StreetNumber"].ToString() ?? "-";
                            resultStoredProcedure.City = row["City"].ToString() ?? "-";
                            resultStoredProcedure.ZipCode = row["ZipCode"].ToString() ?? "-";
                            resultStoredProcedure.BuildingName = row["BuildingName"].ToString() ?? "-";
                            resultStoredProcedure.Entrance = row["Entrance"].ToString() ?? "-";
                            resultStoredProcedure.FloorNumber = (int)row["FloorNumber"];
                            resultStoredProcedure.Apartment = (int)row["Apartment"];
                        }

                        // order items list
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            var orderItem = new OrderItemDetailsSP()
                            {
                                OrderItemId = (int)row["OrderItemId"],
                                ProductId = (int)row["ProductId"],
                                Quantity = (int)row["Quantity"],
                                UnitPrice = (decimal)row["UnitPrice"],
                                ProductName = row["ProductName"].ToString() ?? "-",
                                ProductDescription = row["ProductDescription"].ToString() ?? "-",
                                BrandName = row["BrandName"].ToString() ?? "-",
                                CategoryName = row["CategoryName"].ToString() ?? "-",
                                TotalPrice = (decimal)row["TotalPrice"]
                            };

                            resultStoredProcedure.OrderItems.Add(orderItem);
                        }
                    }
                }

                try
                {
                    var orderDetails = _mapper.Map<OrderDetailsDTO>(resultStoredProcedure);
                    var orderAddress = _mapper.Map<AddressDTO>(resultStoredProcedure);
                    var orderItems = _mapper.Map<List<OrderItemDetailsDTO>>(resultStoredProcedure.OrderItems);

                    var orderDetailsPageDTO = new OrderDetailsPageDTO()
                    {
                        OrderDetails = orderDetails,
                        Address = orderAddress,
                        OrderItems = orderItems
                    };

                    return orderDetailsPageDTO;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"----------------------------------------------------------Error with mapping in service: {ex.Message}");
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in spGetOrderSummaryRaport: {ex.Message}");
                return null;
            }
        }
    }
}
